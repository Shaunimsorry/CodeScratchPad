using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class navEngineWheeled : MonoBehaviour
{
    //Wheel Colliders
    public WheelCollider rWheel1;public WheelCollider rWheel2;public WheelCollider rWheel3;public WheelCollider rWheel4;
    public WheelCollider lWheel1;public WheelCollider lWheel2;public WheelCollider lWheel3;public WheelCollider lWheel4;
    //Wheel Geom (For Movement)
    public GameObject rWheelGeo1;public GameObject rWheelGeo2;public GameObject rWheelGeo3;public GameObject rWheelGeo4;
    public GameObject lWheelGeo1;public GameObject lWheelGeo2;public GameObject lWheelGeo3;public GameObject lWheelGeo4;
    //Vehicle Fixed Variables
    public float maxSpeed;public float maxSteer; public float minSteer;public float accTorque; public float breakTorque;public float revTorque;public float breakingDist;
    public float tgtRelAng;public float distToTgt;public float currentSpeed;
    public List<WheelCollider> accWheels;public List<WheelCollider> allWheels;
    public WheelCollider[] forceWheelsArray = new WheelCollider[4];
    public enum _moveState {acclerating, breaking, stopped, none,maxSpeed}
    public _moveState moveState = _moveState.none;
    public Vector3 dest;
    private Vector3 _dest;
    public bool hasMoveDest;
    public GameObject breaklights;

    public void Start()
    {
        hasMoveDest = true;
        breaklights.SetActive(false);

        //ADD FORCE POWER W
        accWheels.Add(rWheel1);
        accWheels.Add(rWheel2);
        accWheels.Add(lWheel1);
        accWheels.Add(lWheel2);
        //ADD ALL WHEELS TO THE LIST
        allWheels.Add(rWheel1);
        allWheels.Add(rWheel2);
        allWheels.Add(rWheel3);
        allWheels.Add(rWheel4);
        allWheels.Add(lWheel1);
        allWheels.Add(lWheel2);
        allWheels.Add(lWheel3);
        allWheels.Add(lWheel4);

        
        forceWheelsArray[0] = rWheel1;
        forceWheelsArray[1] = rWheel2;
        forceWheelsArray[2] = lWheel1;
        forceWheelsArray[3] = lWheel2;
    }

    public void accToMaxSpeed()
    {
        if(currentSpeed > maxSpeed)
        {
            for(int i = 0; i > forceWheelsArray.Length; i++)
            {
                forceWheelsArray[i].motorTorque = accTorque;
                forceWheelsArray[i].brakeTorque = 0;
            }
        }else
        {
            foreach(WheelCollider fw in forceWheelsArray)
            {
                fw.motorTorque = 50;
            }
        }

        // foreach(WheelCollider forceWheel in accWheels)
        // {
        //     Debug.Log("Accelerating!");
        //     if(currentSpeed < maxSpeed)
        //     {
        //         forceWheel.motorTorque = accTorque;
        //         forceWheel.brakeTorque = 0;
        //     }else
        //     {
        //         Debug.Log("Top Speed Reached");
        //         forceWheel.motorTorque = 0;
        //         forceWheel.brakeTorque = 0;
        //     }
        // }
    }

    public void startBreaking()
    {
        foreach(WheelCollider allWheels in allWheels)
        {
            Debug.Log("Breaking!");
            if(currentSpeed > 0)
            {
                breaklights.SetActive(true);
                allWheels.motorTorque = 0;
                allWheels.brakeTorque = breakTorque;   
            }
        }
    }

    public void zeroOutForces()
    {
        foreach(WheelCollider allWheels in allWheels)
        {
            Debug.Log("Killing Forces!");
            if(currentSpeed == 0)
            {
                breaklights.SetActive(false);
                allWheels.motorTorque = 0;
                allWheels.brakeTorque = 0;   
            }
        }
    }


    public void Update()
    {
        if(moveState == _moveState.stopped && currentSpeed < maxSpeed)
        {
            accToMaxSpeed();
        }else
        {
            moveState = _moveState.maxSpeed;
            if(currentSpeed > 0)
            {
                startBreaking();
            }
            else
            {
                zeroOutForces();
                moveState = _moveState.stopped;
            }

        }
        if (hasMoveDest)
        {
            dest = GameObject.Find("Sphere").transform.position;
            tgtRelAng = Vector3.SignedAngle(dest - gameObject.transform.position, gameObject.transform.forward, gameObject.transform.up);
            distToTgt = (dest - gameObject.transform.position).magnitude;
            currentSpeed = Mathf.Round(gameObject.GetComponent<Rigidbody>().velocity.magnitude*4.5f);
        }else
        {
            Debug.Log("No Move Destination");
        }
    }
}
