﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class AI_drivatar : MonoBehaviour
    {

        ANN ann;
        public float visibleDistance = 50;
        public int epochs = 1000;
        public Vector3 offset;
        public GameObject Lidar;
        private CarController m_Car; // the car controller we want to use
        bool trainingDone = false;
        float trainingProgress = 0;
        double sse = 0;
        double lastSSE = 1;


        public bool loadFromFile = true;

        int count=0;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();

        }
        // Use this for initialization
        void Start()
        {
            ann = new ANN(5,2,2,10,0.3);
            if (loadFromFile)
            {
                LoadWeightsFromFile();
                trainingDone = true;
            }
            else
                StartCoroutine(LoadTrainingSet());
        }

        void OnGUI()
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastSSE);
            GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + ann.alpha);
            GUI.Label(new Rect(25, 55, 250, 30), "Trained: " + trainingProgress);
        }

        IEnumerator LoadTrainingSet()
        {

            string path = Application.dataPath + "/training_Data.txt";
            string line;
            if (File.Exists(path))
            {
                int lineCount = File.ReadAllLines(path).Length;
                StreamReader tdf = File.OpenText(path);
                List<double> calcOutputs = new List<double>();
                List<double> inputs = new List<double>();
                List<double> outputs = new List<double>();

                for (int i = 0; i < epochs; i++)
                {
                    //set file pointer to beginning of file
                    sse = 0;
                    tdf.BaseStream.Position = 0;
                    string currentWeights = ann.PrintWeights();
                    while ((line = tdf.ReadLine()) != null)
                    {
                        string[] data = line.Split(',');
                        //if nothing to be learned ignore this line
                        float thisError = 0;
                        if (System.Convert.ToDouble(data[5]) != 0 && System.Convert.ToDouble(data[6]) != 0)
                        {
                            inputs.Clear();
                            outputs.Clear();
                            inputs.Add(System.Convert.ToDouble(data[0]));
                            inputs.Add(System.Convert.ToDouble(data[1]));
                            inputs.Add(System.Convert.ToDouble(data[2]));
                            inputs.Add(System.Convert.ToDouble(data[3]));
                            inputs.Add(System.Convert.ToDouble(data[4]));

                            double o1 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[5]));
                            outputs.Add(o1);
                            double o2 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[6]));
                            outputs.Add(o2);

                            calcOutputs = ann.Train(inputs, outputs);
                            thisError = ((Mathf.Pow((float)(outputs[0] - calcOutputs[0]), 2) +
                                Mathf.Pow((float)(outputs[1] - calcOutputs[1]), 2))) / 2.0f;
                        }
                        sse += thisError;
                    }
                    trainingProgress = (float)i / (float)epochs;
                    sse /= lineCount;

                    //if sse isn't better then reload previous set of weights
                    //and decrease alpha
                    if (lastSSE < sse)
                    {
                        ann.LoadWeights(currentWeights);
                        ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f, 0.01f, 0.9f);
                    }
                    else //increase alpha
                    {
                        ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f, 0.01f, 0.9f);
                        lastSSE = sse;
                    }

                    yield return null;
                }

            }
            trainingDone = true;
            SaveWeightsToFile();
        }

        void SaveWeightsToFile()
        {
            string path = Application.dataPath + "/weights.txt";
            StreamWriter wf = File.CreateText(path);
            wf.WriteLine(ann.PrintWeights());
            wf.Close();
        }

        void LoadWeightsFromFile()
        {
            string path = Application.dataPath + "/weights.txt";
            StreamReader wf = File.OpenText(path);

            if (File.Exists(path))
            {
                string line = wf.ReadLine();
                ann.LoadWeights(line);
            }
        }

        float Map(float newfrom, float newto, float origfrom, float origto, float value)
        {
            if (value <= origfrom)
                return newfrom;
            else if (value >= origto)
                return newto;
            return (newto - newfrom) * ((value - origfrom) / (origto - origfrom)) + newfrom;
        }

        float Round(float x)
        {
            return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
        }

        void FixedUpdate()
        {
            if (!trainingDone) return;

            List<double> calcOutputs = new List<double>();
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, this.transform.forward * visibleDistance, Color.green);
            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, this.transform.right * visibleDistance, Color.green);
            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, -this.transform.right * visibleDistance, Color.green);

            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, Quaternion.AngleAxis(45, Lidar.transform.up) * -this.transform.right * visibleDistance, Color.green);
            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, Quaternion.AngleAxis(-45, Lidar.transform.up) * this.transform.right * visibleDistance, Color.green);



            //raycasts
            RaycastHit hit;
            float fDist = 0, rDist = 0, lDist = 0, r45Dist = 0, l45Dist = 0;

            //forward
            if (Physics.Raycast(Lidar.GetComponent<Transform>().position + offset, this.transform.forward, out hit, visibleDistance))
            {
                if (hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag == "Ground")
                {
                    fDist = 1 - Round(hit.distance / visibleDistance);
                    Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, this.transform.forward * visibleDistance, Color.red);

                }

            }

            //right
            if (Physics.Raycast(Lidar.GetComponent<Transform>().position + offset, this.transform.right, out hit, visibleDistance))
            {

                if (hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag == "Ground")
                {
                    rDist = 1 - Round(hit.distance / visibleDistance);
                    Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, this.transform.right * visibleDistance, Color.red);
                }
            }

            //left
            if (Physics.Raycast(Lidar.GetComponent<Transform>().position + offset, -this.transform.right, out hit, visibleDistance))
            {

                if (hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag == "Ground")
                {
                    lDist = 1 - Round(hit.distance / visibleDistance);
                    Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, -this.transform.right * visibleDistance, Color.red);
                }
            }

            //right 45
            if (Physics.Raycast(Lidar.GetComponent<Transform>().position + offset,
                                Quaternion.AngleAxis(-45, Vector3.up) * this.transform.right, out hit, visibleDistance))
            {

                if (hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag == "Ground")
                {
                    r45Dist = 1 - Round(hit.distance / visibleDistance);
                    Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, Quaternion.AngleAxis(-45, Lidar.transform.up) * this.transform.right * visibleDistance, Color.red);
                }
            }

            //left 45
            if (Physics.Raycast(Lidar.GetComponent<Transform>().position + offset,
                                Quaternion.AngleAxis(45, Vector3.up) * -this.transform.right, out hit, visibleDistance))
            {

                if (hit.collider.gameObject.tag == "Wall" || hit.collider.gameObject.tag == "Ground")
                {
                    l45Dist = 1 - Round(hit.distance / visibleDistance);
                    Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, Quaternion.AngleAxis(45, Lidar.transform.up) * -this.transform.right * visibleDistance, Color.red);
                }
            }

            inputs.Add(fDist);
            inputs.Add(rDist);
            inputs.Add(lDist);
            inputs.Add(r45Dist);
            inputs.Add(l45Dist);
            outputs.Add(0);
            outputs.Add(0);
            calcOutputs = ann.CalcOutput(inputs, outputs);
            float translationInput = Map(-1, 1, 0, 1, (float)calcOutputs[0]);
            float rotationInput = Map(-1, 1, 0, 1, (float)calcOutputs[1]);

            Debug.Log(fDist+","+ rDist +","+ lDist +"," + r45Dist +","+l45Dist+ "fwd:" + translationInput + ",steer:" + rotationInput);
            //this.transform.Translate(0, 0, translation);
            //this.transform.Rotate(0, rotation, 0);
            float handbrake = Input.GetAxis("Jump");

            float h = Input.GetAxis("Horizontal");

            m_Car.Move(rotationInput + h, translationInput, translationInput, handbrake);

                
        }
    }
}
