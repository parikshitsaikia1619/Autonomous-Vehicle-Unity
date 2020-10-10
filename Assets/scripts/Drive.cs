using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Drive : MonoBehaviour {

	public float speed = 200.0F;
    public float rotationSpeed = 100.0F;
    public float visibleDistance = 50.0f;
    public Vector3 offset;
    List<string> collectedTrainingData = new List<string>();
    StreamWriter tdf;

    void Start()
    {
    	string path = Application.dataPath + "/trainingData.txt";
    	tdf = File.CreateText(path);
    }

    void OnApplicationQuit()
    {
    	foreach(string td in collectedTrainingData)
        {
        	tdf.WriteLine(td);
        }
        tdf.Close();
    }

    float Round(float x) 
    {  	
     	return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
 	}

    void Update() {
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");
        float translation = Time.deltaTime * speed * translationInput;
        float rotation = Time.deltaTime * rotationSpeed * rotationInput;

        //transform.Translate(0, 0, translation);
        //transform.Rotate(0, rotation, 0);

        Debug.DrawRay(transform.position + offset, this.transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position + offset, this.transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position + offset, -this.transform.right * visibleDistance, Color.red);
        //look left
        Debug.DrawRay(transform.position + offset, Quaternion.AngleAxis(45, Vector3.up) * -this.transform.right * visibleDistance, Color.green);
        //look right
        Debug.DrawRay(transform.position + offset, Quaternion.AngleAxis(-45, Vector3.up) * this.transform.right * visibleDistance, Color.green);

		//raycasts
        RaycastHit hit;
        float fDist = 0, rDist = 0, 
                      lDist = 0, r45Dist = 0, l45Dist = 0; 

        //forward
        if (Physics.Raycast(transform.position + offset, this.transform.forward, out hit, visibleDistance))
        {
            fDist = 1 - Round(hit.distance/visibleDistance);
        }  

        //right
        if (Physics.Raycast(transform.position + offset, this.transform.right, out hit, visibleDistance))
        {
            rDist = 1 - Round(hit.distance/visibleDistance);
        }

        //left
        if (Physics.Raycast(transform.position + offset, -this.transform.right, out hit, visibleDistance))
        {
            lDist = 1 - Round(hit.distance/visibleDistance);
        }

        //right 45
		if (Physics.Raycast(transform.position + offset, 
			                Quaternion.AngleAxis(-45, Vector3.up) * this.transform.right, out hit, visibleDistance))
		{
            r45Dist = 1 - Round(hit.distance/visibleDistance);
		}

        //left 45
        if (Physics.Raycast(transform.position + offset, 
        	                Quaternion.AngleAxis(45, Vector3.up) * -this.transform.right, out hit, visibleDistance))
        {
            l45Dist = 1 - Round(hit.distance/visibleDistance);
        }  

        string td = fDist+","+rDist+","+lDist+","+r45Dist+","+l45Dist;

        if (translationInput != 0 && rotationInput != 0)
        {
            if (!collectedTrainingData.Contains(td))
            {
                collectedTrainingData.Add(td);
            }
        }

        //Debug.Log(td);
    }

}
