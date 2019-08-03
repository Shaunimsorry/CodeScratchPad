using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;

public class testHTTP : MonoBehaviour
{
    public string add = "https://jsonplaceholder.typicode.com/todos/1";
    public string responseCode;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(testPage(add));
    }

    public IEnumerator testPage(string add)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(add))
        {
            yield return request.SendWebRequest();
            responseCode = request.responseCode.ToString();
            var jsondata = request.downloadHandler.data;
            Debug.Log(jsondata);
            var jsonString = System.Text.Encoding.UTF8.GetString(jsondata);
            Debug.Log(jsonString.Length);
            var final = JsonUtility.FromJson<RootObject>(jsonString);
            Debug.Log(final.ToString());
        }
    }
}

[System.Serializable]
public class RootObject
{
    public int userId;
    public int id;
    public string title;
    public bool completed;
}
