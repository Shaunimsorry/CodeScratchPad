using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;

public class readRaycast : MonoBehaviour
{
    public Text rcState;
    public Text rcHit;
    public Text _confidence;
    public ControllerConnectionHandler _controlerHandler;
    public GameObject physicalController;
    public GameObject locator;

    public void readCast(MLWorldRays.MLWorldRaycastResultState state, RaycastHit rayhit, float confidence)
    {
        rcState.text = state.ToString();
        rcHit.text = rayhit.point.ToString();
        _confidence.text = confidence.ToString();

        locator.transform.position = rayhit.point;
        locator.transform.LookAt(rayhit.point + rayhit.point);
    
    }
}
