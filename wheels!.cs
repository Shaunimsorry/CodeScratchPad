using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class navEngineWheeled : MonoBehaviour
{
    public WheelCollider rWheel;
    public WheelCollider lWheel;
    public GameObject rWheelGeo;
    public GameObject lWheelGeo;
    
    public WheelCollider rWheel2;
    public WheelCollider lWheel2;
    public GameObject rWheelGeo2;
    public GameObject lWheelGeo2;
    public float maxSteer = 50f;
    public float CurrentRpm;
    public float topSpeed;

    public GameObject target;


    public void Update()
    {
        CurrentRpm = rWheel.rpm + lWheel.rpm/2;
        if(CurrentRpm < topSpeed)
        {
             rWheel.motorTorque = 50f;
             lWheel.motorTorque = 50f;
             rWheel2.motorTorque = 50f;
             lWheel2.motorTorque = 50f;
        }else
        {
            rWheel.motorTorque = 0f;
            lWheel.motorTorque = 0f;
            rWheel2.motorTorque = 0f;
            lWheel2.motorTorque = 0f;
        }

        var tgtRelativePos = gameObject.transform.InverseTransformPoint(target.transform.position);
        var streerTest = tgtRelativePos.x / tgtRelativePos.magnitude * maxSteer;

        rWheel.steerAngle = streerTest;
        lWheel.steerAngle = streerTest;

        rWheel2.steerAngle = streerTest*0.5f;
        lWheel2.steerAngle = streerTest*0.5f;
        
        Vector3 rWheelLook;
        Quaternion rWheelRot;

        rWheel.GetWorldPose(out rWheelLook,out rWheelRot);

        rWheelGeo.transform.rotation = rWheelRot;
        rWheelGeo.transform.position = rWheelLook;

        Vector3 lWheelLook;
        Quaternion lWheelRot;

        lWheel.GetWorldPose(out lWheelLook,out lWheelRot);

        lWheelGeo.transform.rotation = lWheelRot;
        lWheelGeo.transform.position = lWheelLook;

        Vector3 rWheelLook2;
        Quaternion rWheelRot2;

        rWheel2.GetWorldPose(out rWheelLook2,out rWheelRot2);

        rWheelGeo2.transform.rotation = rWheelRot2;
        rWheelGeo2.transform.position = rWheelLook2;

        Vector3 lWheelLook2;
        Quaternion lWheelRot2;

        lWheel2.GetWorldPose(out lWheelLook2,out lWheelRot2);

        lWheelGeo2.transform.rotation = lWheelRot2;
        lWheelGeo2.transform.position = lWheelLook2;
        
    }
}
