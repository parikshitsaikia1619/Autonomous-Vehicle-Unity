using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;




public class ModCarComponent : MonoBehaviour
{

  
    [Header("Lights")]
    public bool frontLightsOn;
    public bool brakeEffectsOn;
    [Space(5)]
    public GameObject brakeEffects;
    public GameObject frontLightEffects;
   
    [Header("Needles")]
 
    public Vector2 SpeedNeedleRotateRange = Vector3.zero;
    private Vector3 SpeedEulers = Vector3.zero;
    public Transform RpmNeedle;
    public Vector2 RpmNeedleRotateRange = Vector3.zero;
    private Vector3 RpmdEulers = Vector3.zero;
    public float _NeedleSmoothing = 1.0f;
    public Transform steeringWheel;

    private float rotateNeedles = 0.0f;

    public float rot_speed = 100.0f;

    public GameObject wheels;

    void Start()
    {

        frontLightsOn = true;
        brakeEffectsOn = false;
        if (RpmNeedle) RpmdEulers = RpmNeedle.localEulerAngles;

    }


    void FixedUpdate()
    {
      
        if (RpmNeedle)
        {
            Vector3 temp = new Vector3(RpmdEulers.x, RpmdEulers.y, Mathf.Lerp(RpmNeedleRotateRange.x, RpmNeedleRotateRange.y, (rotateNeedles)));
            RpmNeedle.localEulerAngles = Vector3.Lerp(RpmNeedle.localEulerAngles, temp, Time.deltaTime * _NeedleSmoothing);
        }

        if (steeringWheel != null)
        {

            Vector3 eulers = steeringWheel.localRotation.eulerAngles;
            Vector3 wheel_eular = wheels.transform.localRotation.eulerAngles;
            eulers.z = -wheel_eular.y*rot_speed;
          
            steeringWheel.localRotation = Quaternion.Slerp(steeringWheel.localRotation, Quaternion.Euler(eulers), Time.deltaTime * 3.5f);
        

        }




        if (Input.GetAxis("Vertical") < 0)
            brakeEffectsOn = true;
        else
            brakeEffectsOn = false;

        TurnOnFrontLights();
        TurnOnBackLights();

    }

    public void TurnOnFrontLights()
    {
        if (frontLightsOn)
        {
            frontLightEffects.SetActive(true);
            rotateNeedles += Time.deltaTime;
        }
        else
        {
            frontLightEffects.SetActive(false);
            rotateNeedles -= Time.deltaTime;
        }
    }

    public void TurnOnBackLights()
    {
        if (brakeEffectsOn)
        {
            brakeEffects.SetActive(true);
        }
        else
        {
            brakeEffects.SetActive(false);
        }
    }


}


