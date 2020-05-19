using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Unity.Utilities;
using Mapbox.Utils.JsonConverters;


public class MapboxApI : MonoBehaviour
{
    //API Details
    public string responseCode;
    public string jsonOutput;
    public string dataset_id = "cka0vudxx13p72smuavafd1um";
    public string access_token = "pk.eyJ1IjoidGFua2J1c3RhIiwiYSI6ImNrN3BhYjdvZTAzbHMza3VmMmhhbGxtc3YifQ.Z8ohtdn7-BsOzeznsXm3EQ";
    public string secret_token = "sk.eyJ1IjoidGFua2J1c3RhIiwiYSI6ImNrYWQ3MjN6ODFqMWMzNHM5NzM3bm40ajgifQ.WEZKnrork6sshSBo1aPcjA";
    public string userName = "tankbusta";
    public void Start()
    {
        //StartCoroutine(listLandmarks());
        StartCoroutine(createLandmark());

    }

    public IEnumerator listLandmarks()
    {
        string endpoint_list = "https://api.mapbox.com/datasets/v1/"+userName+"/"+dataset_id+"/"+"features?"+"access_token="+access_token;
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint_list))
        {
            yield return www.SendWebRequest();
            responseCode = www.responseCode.ToString();
            //var r = JsonUtility.FromJson<MapBoxGEOJSON_GET>(www.downloadHandler.text);
            //Debug.Log(r.features[0].geometry.coordinates[0]);
        }
    }

    public IEnumerator createLandmark()
    {   
        double x = 51.186553490730;
        double y = 21.497919311020;

        string feature_id = "d470b6133258df834ed36fc0c3ec0";

        mapboxFeatureClass testLandMark = new mapboxFeatureClass();

        Properties featureProperties = new Properties();
        Geometry featureGeometry = new Geometry();
        List<double> featurecoordinates = new List<double>();

        testLandMark.properties = featureProperties;
        testLandMark.geometry = featureGeometry;
        testLandMark.geometry.coordinates = featurecoordinates;


        testLandMark.id = feature_id;
        testLandMark.type = "Feature";
        testLandMark.properties.name = "ThePlace";
        testLandMark.geometry.coordinates.Add(x);
        testLandMark.geometry.coordinates.Add(y);
        testLandMark.geometry.type = "Point";
        

        //This worked
        //string workingoutput = "{\"type\":\"Feature\",\"properties\":{\"name\":\"Null Island\"},\"geometry\":{\"type\":\"Point\",\"coordinates\":[-2.3626667261123657,53.45663959042344]}}";
        jsonOutput= JsonUtility.ToJson(testLandMark);

        //string endpoint_create = "https://api.mapbox.com/datasets/v1/tankbusta/cka0vudxx13p72smuavafd1um/features/236d470b6133258df834ed36fc0c3ec0?access_token=sk.eyJ1IjoidGFua2J1c3RhIiwiYSI6ImNrYWQ3MjN6ODFqMWMzNHM5NzM3bm40ajgifQ.WEZKnrork6sshSBo1aPcjA";
        string endpoint_create = "https://api.mapbox.com/datasets/v1/tankbusta/cka0vudxx13p72smuavafd1um/features/"+feature_id+"/?access_token=sk.eyJ1IjoidGFua2J1c3RhIiwiYSI6ImNrYWQ3MjN6ODFqMWMzNHM5NzM3bm40ajgifQ.WEZKnrork6sshSBo1aPcjA";
        UnityWebRequest www=UnityWebRequest.Put(endpoint_create,jsonOutput);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        responseCode = www.responseCode.ToString();
        
        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error: "+www.error);
        }else
        {
            Debug.Log("Passed "+www.downloadHandler.text);
        }
    }
}
