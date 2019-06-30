using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class navEngineWheeled : MonoBehaviour
{
    public WheelCollider rWheel;
    public WheelCollider lWheel;
    public GameObject rWheelGeo;
    public GameObject lWheelGeo;
    public float targetRelAngle;
    public float distToTgt;
    public float maxSteer = 50f;
    public float WheelRPM;
    public float currentSpeed;
    public float topSpeed;
    public float steerSpeed = 25f;
    public GameObject target;
    public GameObject breakights;

    private Vector3 lastpos;
    private bool turnedToTgt;

    private bool isStopped;
    private bool isBreaking;
    private bool canTurnToTgt;
    private bool kturning;
    private bool readytoKTurn;
    private float rWheelRevSteerAngle;
    private float lWheelRevStreetAngle;
    public void Start()
    {
        breakights.SetActive(false);
        isBreaking = false;
        isStopped = false;
        kturning = false;
        readytoKTurn = false;
    }

    public void movetoTgt()
    {
        Debug.Log("Moving to tgt");
        Vector3 rWheelLook;
        Quaternion rWheelRot;
        Vector3 lWheelLook;
        Quaternion lWheelRot;
        var tgtRelativePos = gameObject.transform.InverseTransformPoint(target.transform.position);
        var steerAngle = tgtRelativePos.x / tgtRelativePos.magnitude * maxSteer;

        if (currentSpeed < topSpeed)
        {
            rWheel.motorTorque = 250f;
            lWheel.motorTorque = 250f;
            rWheel.brakeTorque = 0F;
            lWheel.brakeTorque = 0F;
        }
        else
        {
            rWheel.motorTorque = 0f;
            lWheel.motorTorque = 0f;
        }   
        
        rWheel.steerAngle = steerAngle;
        lWheel.steerAngle = steerAngle;
        

        rWheel.GetWorldPose(out rWheelLook, out rWheelRot);

        rWheelGeo.transform.rotation = rWheelRot;
        rWheelGeo.transform.position = rWheelLook;

        lWheel.GetWorldPose(out lWheelLook, out lWheelRot);

        lWheelGeo.transform.rotation = lWheelRot;
        lWheelGeo.transform.position = lWheelLook;
    }
    public void reOrientToTgt()
    {
        if(currentSpeed > 0 && !canTurnToTgt && !kturning)
        {
            Debug.Log("Killing speed to re-orient");
            breakights.SetActive(true);
            rWheel.motorTorque = 0f;
            lWheel.motorTorque = 0f;
            rWheel.brakeTorque = 50F;
            lWheel.brakeTorque = 50F;

            //Store The Correct Turn Here
            rWheelRevSteerAngle = rWheel.steerAngle *-1;
            lWheelRevStreetAngle = lWheel.steerAngle *-1;
        }else
        {

            Debug.Log("Breaking Complete");
            breakights.SetActive(false);
            rWheel.motorTorque = 0f;
            lWheel.motorTorque = 0f;
            rWheel.brakeTorque = 0f;
            lWheel.brakeTorque = 0f;
            kturning = true;
        }

        if(kturning)
        {
            //Debug.Log("Streering Angle Is"+rWheel.steerAngle);
            //Debug.Log("Rev Angle Is"+rWheelRevSteerAngle);


            Debug.Log("Starting K Turn");
            if(rWheel.steerAngle <= rWheelRevSteerAngle&& lWheel.steerAngle <= lWheelRevStreetAngle)
            {
                Debug.Log("Correcting Steering!");
                rWheel.steerAngle = rWheel.steerAngle + steerSpeed*Time.deltaTime;
                lWheel.steerAngle = lWheel.steerAngle + steerSpeed*Time.deltaTime;
                readytoKTurn = false;
            }else
            {
                readytoKTurn = true;
            }

            if(readytoKTurn)
            {
                Debug.Log("Backing Up");
                rWheel.motorTorque = -200;
                rWheel.brakeTorque = 0;
                lWheel.motorTorque = -200;
                lWheel.brakeTorque = 0;
            }
        }


    }
    public void Update()
    {
        if(targetRelAngle < maxSteer)
        {
            canTurnToTgt = true;
        }else
        {
            canTurnToTgt = false;
        }
        if(canTurnToTgt)
        {
            movetoTgt();
        }else
        {
            reOrientToTgt();
        }

        //RPM CALCULATION THINGS
        WheelRPM = rWheel.rpm + lWheel.rpm / 2;
        var mps = (lastpos - gameObject.transform.position).magnitude / Time.deltaTime;
        if (mps > 0)
        {
            currentSpeed = Mathf.Round(mps * 3.6f);
        }
        lastpos = gameObject.transform.position;

        //Turning Things
        targetRelAngle = Vector3.SignedAngle(target.transform.position - gameObject.transform.position, gameObject.transform.forward, gameObject.transform.up);
        distToTgt = (target.transform.position - gameObject.transform.position).magnitude;



    }
}
