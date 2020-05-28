using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mapbox.Utils;
using Mapbox.Map;
using UnityARInterface;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Location;
using UnityEngine.UI;
using TMPro;

public class MapboxApI : MonoBehaviour
{

    //Interface Into Locationprovider
    public LocationProviderFactory locationProviderFactoryLink;
    public ARFocusSquare userFocusSquare;
    public float landmarkSpawnDistance = 100.0f; 

    public Text UpdateInfoDebug;
    public string hudDebug00;
    public List<mapboxFeatureClass> dynamicFeatureList;

    //LandMark Spawning Variables
    public InputField TXTInput_LandMarkName;
    public string landmarkLogoCode;
    public Vector3 landmarkScale;
    public GameObject landMarkPrefab;
    //The Landmark the user is looking at (deduced via raycasting)
    public GameObject userLookLandMark;



    //API Details
    public string responseCode;
    public string jsonOutput;
    public string dataset_id = "cka0vudxx13p72smuavafd1um";
    public string access_token = "pk.eyJ1IjoidGFua2J1c3RhIiwiYSI6ImNrN3BhYjdvZTAzbHMza3VmMmhhbGxtc3YifQ.Z8ohtdn7-BsOzeznsXm3EQ";
    public string secret_token = "sk.eyJ1IjoidGFua2J1c3RhIiwiYSI6ImNrYWQ3MjN6ODFqMWMzNHM5NzM3bm40ajgifQ.WEZKnrork6sshSBo1aPcjA";
    public string mapbox_username = "tankbusta";
    

    //Setting Up Dyanmic Lists
    public mapboxFeatureSet RetrievedFeatureList;

    //Building Live Landmark Spawning
    public List<GameObject> LiveLandMarks;

    public LMICreate LandMarkCreateGUI;
    public LMIInfo LandMarkInfoGUI;
    public Button userProfileButton;
    public GameObject userProfilePlaceHolder;
    public bool LandmarkWindow = false;
    public string playerDatabaseJsonData;
    public string username;
    public int userlikes;
    public int userLandmarks;
    public int userLvl;

    public TextMeshProUGUI HUD_Username;
    public TextMeshProUGUI HUD_Landmarks;
    public TextMeshProUGUI HUD_Likes;



    //Start!
    public void Start()
    {
        //Setup JSON PATH (Andriod Only)
        playerDatabaseJsonData = PullJsonPath();
        //Setup Username From Previous Screen
        LocateUserJSONData(MainMenuController.userName);
        //Activate The Mapbox API and Pulldown all the data
        StartCoroutine(listLandmarks());
        //Populate The Dynamic List With Landmarks 100 Clicks From GPS every 5 Seconds
        StartCoroutine(populateDynamicList(landmarkSpawnDistance));
        //Deploy and manage all landmarks from the lists
    }

    public void Update()
    {
        string lookatV3 = userFocusSquare.focusSquareRayCastHitVector.ToString();
        string lookatV2 = locationProviderFactoryLink.mapManager.WorldToGeoPosition(userFocusSquare.focusSquareRayCastHitVector).ToString();
        string totalFeatures = RetrievedFeatureList.features.Count.ToString();
        string dynamicFeatures = dynamicFeatureList.Count.ToString();
        string livelandmarks = LiveLandMarks.Count.ToString();
        UpdateInfoDebug.text = "LAV3: "+lookatV3+"\n"+"LAV2: "+lookatV2+"\n"+"TF: "+totalFeatures+"\n"+"DF: "+dynamicFeatures+"\n"+"LL: "+livelandmarks+"\n"+"HUD Debug00: "+hudDebug00;

        checkUserTouchOnLandmark();
    }

    public void CreateNewLandMarkAtLocation()
    {
        Vector2d landmarkLocation = locationProviderFactoryLink.mapManager.WorldToGeoPosition(userFocusSquare.focusSquareRayCastHitVector);
        Vector3 landmarkDeploylocation = userFocusSquare.focusSquareRayCastHitVector;
        float landmarkHeight = landmarkDeploylocation.y;
        string feature_id = generateFeatureID();
        
        //bEING tESTED
        string LandMarkName = LandMarkCreateGUI.LandMarkName.text;
        string LandMarkLogoShape = LandMarkCreateGUI.LandmarkLogoShape;

        StartCoroutine(createLandmark(landmarkLocation,LandMarkName,feature_id,landmarkHeight,LandMarkLogoShape, username));

        GameObject createdLandmark = GameObject.Instantiate(landMarkPrefab,landmarkDeploylocation,transform.rotation);

        createdLandmark.GetComponent<ARLandMarkInternalController>().landmarkLogo = LandMarkLogoShape;
        createdLandmark.GetComponent<ARLandMarkInternalController>().stringlandmarkText = LandMarkCreateGUI.LandMarkName.text;
        createdLandmark.GetComponent<ARLandMarkInternalController>().stringlandmarkCreator = username;
        createdLandmark.GetComponent<ARLandMarkInternalController>().stringlandMarkLikes = "0";
        //End Testing

        createdLandmark.name = "Landmark_"+feature_id.ToString();
        createdLandmark.transform.localScale = landmarkScale;
        
        //Add to LiveList
        LiveLandMarks.Add(createdLandmark);
        LandMarkCreateGUI.hideCreateMenu();

    }

    public void SpawnExistingLandMarkIntoScene(string feature_id, mapboxFeatureClass feature)
    {
        Vector2d existingLandMarkLocation;
        existingLandMarkLocation.y = feature.geometry.coordinates[0];
        existingLandMarkLocation.x = feature.geometry.coordinates[1];
        Vector3 existingLandMarkUnityLocation = locationProviderFactoryLink.mapManager.GeoToWorldPosition(existingLandMarkLocation);
        
        //FIND A BETTER WAY TO DO THIS #TODO
        existingLandMarkUnityLocation.y = -1;
       
        GameObject existingLandMark = GameObject.Instantiate(landMarkPrefab,existingLandMarkUnityLocation, landMarkPrefab.transform.rotation);
        existingLandMark.name = "Landmark_"+feature_id.ToString();
        existingLandMark.transform.localScale = landmarkScale;

        //Preload physical look values from the data on the cloud
        existingLandMark.GetComponent<ARLandMarkInternalController>().landmarkLogo = feature.properties.logoShape;
        existingLandMark.GetComponent<ARLandMarkInternalController>().stringlandmarkText = feature.properties.name;
        existingLandMark.GetComponent<ARLandMarkInternalController>().stringlandmarkCreator = feature.properties.creator;
        existingLandMark.GetComponent<ARLandMarkInternalController>().landMarkID = feature.properties.landmarkID;
        existingLandMark.GetComponent<ARLandMarkInternalController>().stringlandMarkLikes = feature.properties.likes.ToString();

        //Add to LiveList
        LiveLandMarks.Add(existingLandMark);
    }

    public float landMarkDistance(mapboxFeatureClass inputFeature, Vector3 LookAtRaycast)
    {
        //This function calculates the distance between the user's focus square and a particular feature
        Vector3 featureVector3;
        Vector2d featureVector2d;

        //Need to check the latlon or lonlat here
        //Appears That X is Lat and Y is Lon
        //JSON 0 is Lon and 1 is Lat
        //Still to be confirmed
        featureVector2d.x = inputFeature.geometry.coordinates[1];
        featureVector2d.y = inputFeature.geometry.coordinates[0];
        featureVector3 = locationProviderFactoryLink.mapManager.GeoToWorldPosition(featureVector2d);
        return Vector3.Distance(LookAtRaycast,featureVector3);

    }
    public string generateFeatureID()
    {
        string generatedFeatureID = "";
        const string glyphs = "abcdefghijklmnopqrstuvwxyz123456789";

        //added one more here to make it 32
        //string referenceMapBoxString = "1d470b6133258df834ed36fc0c3ec0";
        for(int i = 0; i<32; i++)
        {
            generatedFeatureID += glyphs[Random.Range(0,glyphs.Length)];
        }
        return generatedFeatureID;
    }
    public IEnumerator listLandmarks()
    {
        Debug.Log("Listing Landmarks");
        string endpoint_list = "https://api.mapbox.com/datasets/v1/"+mapbox_username+"/"+dataset_id+"/"+"features?"+"access_token="+access_token;
        UnityWebRequest www=UnityWebRequest.Get(endpoint_list);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        responseCode = www.responseCode.ToString();
        string downloadeddata = www.downloadHandler.text;
        Debug.Log(downloadeddata);
        RetrievedFeatureList = JsonUtility.FromJson<mapboxFeatureSet>(downloadeddata);
        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error: "+www.error);
        }else
        {
            Debug.Log("Passed "+www.downloadHandler.text);
        }
    }
    public IEnumerator createLandmark(Vector2d landmarkLocation, string LandMarkName, string feature_id, float landmarkHeight, string landmarkLogoShape, string creatorName)
    {   
        //Prepare Dynamic Placeholders
        mapboxFeatureClass testLandMark = new mapboxFeatureClass();
        Properties featureProperties = new Properties();
        Geometry featureGeometry = new Geometry();
        List<double> featurecoordinates = new List<double>();
        testLandMark.properties = featureProperties;
        testLandMark.geometry = featureGeometry;
        testLandMark.geometry.coordinates = featurecoordinates;
        var userFocusVector2D = locationProviderFactoryLink.mapManager.WorldToGeoPosition(userFocusSquare.focusSquareRayCastHitVector);

        //Fill in the landmark Data
        testLandMark.id = feature_id;
        testLandMark.type = "Feature";
        testLandMark.properties.name = LandMarkName;
        testLandMark.geometry.coordinates.Add(landmarkLocation.y);
        testLandMark.geometry.coordinates.Add(landmarkLocation.x);
        testLandMark.geometry.type = "Point";
        testLandMark.properties.creator = creatorName;
        testLandMark.properties.likes = 0;
        testLandMark.properties.landmarkID = feature_id;
        testLandMark.properties.logoShape = landmarkLogoShape;

        //Add the to spawn lists
        dynamicFeatureList.Add(testLandMark);

        //Final JSON Conversion
        jsonOutput= JsonUtility.ToJson(testLandMark);

        //Start Sending the landmark to MBApi
        string endpoint_create = "https://api.mapbox.com/datasets/v1/tankbusta/"+dataset_id+"/features/"+feature_id+"/?access_token="+secret_token;
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
    public IEnumerator populateDynamicList(float maxDistance)
    {
        Debug.Log("Populating List!");
        //Periodically Populate the list and exclude if the distance is larger than max distance
        //This should eventually become a city or area list << TODO
        //So the larger ideas is 
        foreach(mapboxFeatureClass i in RetrievedFeatureList.features)
        {
            Debug.Log("Checking Max D");
            float distance = landMarkDistance(i,userFocusSquare.focusSquareRayCastHitVector);
            if (distance < maxDistance)
            {
                Debug.Log("Found Landmarks Undermax D");
                //Check if the feature is live or in the dynaamic list
                if(!dynamicFeatureList.Contains(i))
                {
                    Debug.Log("Feauture In List!");
                    //Not in game or dynamic list spawn the existing object
                    dynamicFeatureList.Add(i);
                    SpawnExistingLandMarkIntoScene(i.properties.landmarkID,i);
                }else
                {
                    Debug.Log("Correcting for error");
                    //Check if the object is in the lists but not in the scene
                    GameObject searchResult = GameObject.Find("Landmark_"+i.properties.landmarkID.ToString());
                    if (!searchResult)
                    {
                        if(searchResult = null)
                        {
                            dynamicFeatureList.Remove(i);
                        }
                    }
                }
            }else
            {
                //Object is too far remove and clean from lists
                dynamicFeatureList.Remove(i);
                string LandMarkToDestroyName = "Landmark_"+i.properties.landmarkID.ToString();
                GameObject.Destroy(GameObject.Find(LandMarkToDestroyName));
                //You can optimize this
                LiveLandMarks.Remove(GameObject.Find(LandMarkToDestroyName));
            }
        }
        StartCoroutine(updateAllSpawnedLandmarkLocations());
        yield return new WaitForSeconds(5.0f);
        StartCoroutine(populateDynamicList(landmarkSpawnDistance));
    }

    public IEnumerator updateAllSpawnedLandmarkLocations()
    {
        foreach(GameObject i in LiveLandMarks)
        {
            foreach(mapboxFeatureClass feature in dynamicFeatureList)
            {
                if(i.name.Contains(feature.properties.landmarkID))
                {
                    Vector2d reConfirmedVector2D;
                    reConfirmedVector2D.y = feature.geometry.coordinates[0];
                    reConfirmedVector2D.x = feature.geometry.coordinates[1];
                    Vector3 reConfirmedVector3 = locationProviderFactoryLink.mapManager.GeoToWorldPosition(reConfirmedVector2D);
                    
                    //FIND A BETTER WAY TO DO THIS #TODO
                    reConfirmedVector3.y = -1;
                    i.transform.position = reConfirmedVector3;
                }
            }
        }
        yield return null;
    }

    public IEnumerator updateLandMark(string landmarkId, int newlikes)
    {
        mapboxFeatureClass targetFeature = new mapboxFeatureClass();
        foreach(mapboxFeatureClass i in RetrievedFeatureList.features)
        {
            if(i.properties.landmarkID == landmarkId)
            {
                Debug.Log("Found Feature Match: "+landmarkId);
                targetFeature = i;
                Debug.Log(targetFeature.properties.name.ToString());
            }
        }
        targetFeature.properties.likes = newlikes;
        string updatedJson = JsonUtility.ToJson(targetFeature);
        string endpoint_update = "https://api.mapbox.com/datasets/v1/tankbusta/"+dataset_id+"/features/"+targetFeature.properties.landmarkID+"?access_token="+secret_token;
        UnityWebRequest www=UnityWebRequest.Put(endpoint_update,updatedJson);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        responseCode = www.responseCode.ToString();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error: "+www.error);
        }else
        {
            Debug.Log("Updated Landmark!");
            Debug.Log("Passed "+www.downloadHandler.text);
        }
    }

    public GameObject checkUserTouchOnLandmark()
    {
        //Custom Function to detecting if the user hit an existing landmark or something in the livelist ?
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if(LandmarkWindow == false && Input.GetTouch(0).position.y < 1700)
            {
                hudDebug00 = "Raycasting!";
                //Dev: Check if finger touch was detected with no landmark window presenrt
                //Raycast
                RaycastHit hit;
                if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward, out hit))
                {
                    if(hit.collider.gameObject.layer == 13)
                    {
                        //Layer 12 is the landmark layer]
                        hudDebug00 = "Hitting Landmark!";
                        LandMarkInfoGUI.showInfoMenu(hit.collider.gameObject);

                    }else
                    {
                        if(hit.collider.gameObject.layer == 8)
                        {
                            hudDebug00 = "Hitting Floor!";
                            //Hit the floor and no window was open
                            LandMarkCreateGUI.showCreateMenu();
                        }
                    }
                }
            }


        }
        return null;

    }


    public void LocateUserJSONData(string startScreenUserName)
    {
        //read the player data
        // var reader = new System.IO.StreamReader(playerDatabaseJson);
        // string jsonString = reader.ReadToEnd();
        // reader.Close();
        var jsonResult = JsonUtility.FromJson<PlayerSet>(playerDatabaseJsonData);
        PlayerClass targetPlayerClass;

        username = null;
        //Locate and validate the user
        foreach (PlayerClass i in jsonResult.ActivePlayers)
        {
            if(i.playerName == startScreenUserName)
            {
                targetPlayerClass = i;
                username = i.playerName;
                userlikes = i.playerLikes;
                userLvl = i.playerLevel;
                userLandmarks = i.playerLandmarks;
            }else
            {
                if(i.playerName.ToLower() == startScreenUserName.ToLower())
                {
                    targetPlayerClass = i;
                    username = i.playerName;
                    userlikes = i.playerLikes;
                    userLvl = i.playerLevel;
                    userLandmarks = i.playerLandmarks;
                }else
                {
                    if(i.playerName.ToUpper() == startScreenUserName.ToUpper())
                    {
                        targetPlayerClass = i;
                        username = i.playerName;
                        userlikes = i.playerLikes;
                        userLvl = i.playerLevel;
                        userLandmarks = i.playerLandmarks;
                    }
                }
            }
        }

        //Fill in the data
        HUD_Username.text = username;
        HUD_Likes.text = userlikes.ToString();
        HUD_Landmarks.text = userLandmarks.ToString();

    }

    public string PullJsonPath()
    {
        var loadingReq = UnityWebRequest.Get(System.IO.Path.Combine(Application.streamingAssetsPath,"playerData.json"));
        loadingReq.SendWebRequest();
        return loadingReq.downloadHandler.text;
    }



    //THIS IS AN EXPIRED JSON FUNCTION///

    // public void writePlayerData()
    // {
    //     //Pull the current json data
    //     var reader = new System.IO.StreamReader(playerJsonDatabasePath);
    //     string jsonString = reader.ReadToEnd();
    //     reader.Close();
    //     var jsonResult = JsonUtility.FromJson<PlayerSet>(jsonString);

    //     //Add a new player
    //     PlayerClass newPlayer = new PlayerClass();
    //     newPlayer.playerName = "Steph";
    //     newPlayer.playerID = 0003;
    //     newPlayer.playerLikes = 130;
    //     newPlayer.playerLandmarks = 215;
    //     newPlayer.playerLevel = 309;
    //     newPlayer.playerAge = 19;
    //     newPlayer.playerCountry = "China";

    //     jsonResult.ActivePlayers.Add(newPlayer);

    //     var jsonWriteData = JsonUtility.ToJson(jsonResult,true);
    //     System.IO.File.WriteAllText(playerJsonDatabasePath,jsonWriteData);


    //     // PlayerSet test1 = new PlayerSet();
    //     // List<PlayerClass> PlayerList = new List<PlayerClass>();
    //     // PlayerClass newPlayer = new PlayerClass();

    //     // PlayerList.Add(newPlayer);
    //     // test1.ActivePlayers = PlayerList;



    //     // test1.ActivePlayers[0].playerName = "Shaun";
    //     // test1.ActivePlayers[0].playerID = 0001;
    //     // test1.ActivePlayers[0].playerLikes = 200;
    //     // test1.ActivePlayers[0].playerLandmarks = 15;
    //     // test1.ActivePlayers[0].playerLevel = 88;
    //     // test1.ActivePlayers[0].playerAge = 21;
    //     // test1.ActivePlayers[0].playerCountry = "China";

    //     // var jsonData = JsonUtility.ToJson(test1);

    //     // System.IO.File.WriteAllText(playerJsonDatabasePath,jsonData);
    // }
}
