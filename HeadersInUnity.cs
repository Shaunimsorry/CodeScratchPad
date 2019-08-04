using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class listThings : MonoBehaviour
{
    public string responseCode;
    public string temporaryToken;
    

    public void Start()
    {

        StartCoroutine(uploadToGDrive());   
    }

    public IEnumerator uploadToGDrive()
    {
        //Authenticate and wait for authentication
        string endPoint = "https://pastebin.com/api/api_login.php";
        string devKey = "0cf7587f8e888b13ea25e6ad63bec993";
        List<IMultipartFormSection> loginHeaders = new List<IMultipartFormSection>();
        loginHeaders.Add(new MultipartFormDataSection("api_dev_key",devKey));
        loginHeaders.Add(new MultipartFormDataSection("api_user_name","Shaunimsorry"));
        loginHeaders.Add(new MultipartFormDataSection("api_user_password","SHaun123"));
        using (UnityWebRequest www = UnityWebRequest.Post(endPoint,loginHeaders))
        {
            yield return www.SendWebRequest();
            responseCode = www.responseCode.ToString();
            temporaryToken = www.downloadHandler.text.ToString();
            Debug.Log(www.downloadHandler.text.ToString());
            if(temporaryToken != null)
            {
                string endPointBeta = "https://pastebin.com/api/api_post.php";
                List<IMultipartFormSection> postHeaders = new List<IMultipartFormSection>();
                postHeaders.Add(new MultipartFormDataSection("api_dev_key",devKey));
                postHeaders.Add(new MultipartFormDataSection("api_user_key",temporaryToken));
                postHeaders.Add(new MultipartFormDataSection("api_paste_name","Some Title"));
                postHeaders.Add(new MultipartFormDataSection("api_option","paste"));
                postHeaders.Add(new MultipartFormDataSection("api_paste_code","This is some code?"));
                postHeaders.Add(new MultipartFormDataSection("api_paste_expire_date","N"));
                using (UnityWebRequest second = UnityWebRequest.Post(endPointBeta,postHeaders))
                {
                    yield return second.SendWebRequest();
                    Debug.Log("End of program!");
                    Debug.Log(second.downloadHandler.text.ToString());
                }

            }
        }

    }
}
