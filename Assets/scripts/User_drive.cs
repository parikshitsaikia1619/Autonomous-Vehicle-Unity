using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class User_drive : MonoBehaviour
    {
        public float visibleDistance = 50.0f;
        public Vector3 offset;
        public GameObject Lidar;
        List<string> collectedTrainingData = new List<string>();
        StreamWriter tdf;

        private CarController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }

        void Start()
        {
            string path = Application.dataPath + "/training_Data.txt";
            tdf = File.CreateText(path);
        }

        void OnApplicationQuit()
        {
            foreach (string td in collectedTrainingData)
            {
                tdf.WriteLine(td);
            }
            tdf.Close();
        }

        float Round(float x)
        {
            return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
        }

        void Update()
        {
            float translationInput = Input.GetAxis("Vertical");
            float rotationInput = Input.GetAxis("Horizontal");
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");

            m_Car.Move(rotationInput, translationInput, translationInput, handbrake);


            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, this.transform.forward * visibleDistance, Color.green);
            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, this.transform.right * visibleDistance, Color.green);
            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, -this.transform.right * visibleDistance, Color.green);

            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, Quaternion.AngleAxis(45, Lidar.transform.up) * -this.transform.right * visibleDistance, Color.green);
            Debug.DrawRay(Lidar.GetComponent<Transform>().position + offset, Quaternion.AngleAxis(-45, Lidar.transform.up) * this.transform.right * visibleDistance, Color.green);

            //raycasts
            RaycastHit hit;
            float fDist = 0, rDist = 0,
                          lDist = 0, r45Dist = 0, l45Dist = 0;

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

            string td = fDist + "," + rDist + "," + lDist + "," + r45Dist + "," + l45Dist+ "," + Round(translationInput) + "," + Round(rotationInput);

            if (translationInput != 0 && rotationInput != 0)
            {
                if (!collectedTrainingData.Contains(td))
                {
                    collectedTrainingData.Add(td);
                }
            }

            Debug.Log(td);
        }

    }
}
