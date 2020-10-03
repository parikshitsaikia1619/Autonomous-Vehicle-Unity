using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class teleport : MonoBehaviour
{
    public GameObject Ship;
    public GameObject teleport1;
    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.X))
        {
            Ship.transform.SetPositionAndRotation(teleport1.transform.position, teleport1.transform.rotation);
            Debug.Log("TELEPORT!!!");
        }
    }
}
