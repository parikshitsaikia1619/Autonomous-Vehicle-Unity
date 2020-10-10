using UnityEngine;

public class cam_switch : MonoBehaviour
{
    public Camera cam;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    public Camera cam4;


    int count;
    

    public void Show_cam()
    {
        cam.enabled = true;
        cam1.enabled = false;
        cam2.enabled = false;
        cam3.enabled = false;
        cam4.enabled = false;
    }

    public void Show_cam1()
    {
        cam.enabled = false;
        cam1.enabled = true;
        cam2.enabled = false;
        cam3.enabled = false;
        cam4.enabled = false;
    }

    public void Show_cam2()
    {
        cam.enabled = false;
        cam1.enabled = false;
        cam2.enabled = true;
        cam3.enabled = false;
        cam4.enabled = false;
    }

    public void Show_cam3()
    {
        cam.enabled = false;
        cam1.enabled = false;
        cam2.enabled = false;
        cam3.enabled = true;
        cam4.enabled = false;
    }

    public void Show_cam4()
    {
        cam.enabled = false;
        cam1.enabled = false;
        cam2.enabled = false;
        cam3.enabled = false;
        cam4.enabled = true;
    }

    void Start()
    {
        Show_cam();
        count = 0;
    }
    void Update()
    {
        
            /*if(Input.GetKey(KeyCode.C))
            {
                ShowFirstPersonView();
            }

            if (Input.GetKey(KeyCode.X))
            {
                ShowOverheadView();
            }
            */

            if(Input.GetKeyDown("c"))
            {
            count++;
            }

        if(count%5 == 0)
        {
            Show_cam();
        }
        else if (count % 5 == 1)
        {
            Show_cam1();
        }

        else if (count % 5 == 2)
        {
            Show_cam2();
        }

        else if (count % 5 == 3)
        {
            Show_cam3();
        }

        else if (count % 5 == 4)
        {
            Show_cam4();
        }
    }
}