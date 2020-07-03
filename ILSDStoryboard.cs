using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

using UnityEngine.Formats.Alembic.Sdk;
using UnityEngine.Formats.Alembic.Importer;

public class mainStoryController : MonoBehaviour
{
    public GameObject nextButton;
    public GameObject backButton;
    public List<Texture> storyTextures;
    public List<GameObject> storypageList;
    public int storyPages;
    public int currentStoryPage = -1;
    public GameObject pagePrefab;
    public GameObject rightStoryPage;
    public GameObject leftStoryPage;
    public void Start()
    {
        storyPages = storyTextures.Count;
        backButton.SetActive(false);
        preSpawnAllPages();
        currentStoryPage++;
        rightStoryPage = GameObject.Find("StoryPage_"+currentStoryPage.ToString());
        leftStoryPage = null;
     
    }

    public void preSpawnAllPages()
    {
        var currentPreSpawnPage = 0;
        foreach(Texture t in storyTextures)
        {
            GameObject g = GameObject.Instantiate(pagePrefab,pagePrefab.transform.position,pagePrefab.transform.rotation);
            storypageList.Add(g);
            g.name = "StoryPage_"+currentPreSpawnPage.ToString();
            g.GetComponent<pageIdentity>().pageGeo.GetComponent<MeshRenderer>().material.SetTexture("_MainTex",storyTextures[currentPreSpawnPage]);
            currentPreSpawnPage++;
        }
    }
    public void NextPage()
    {
        StartCoroutine(enum_nextPage());  
    }

    public void PreviousPage()
    {
        StartCoroutine(enum_prevPage());
    }

    public IEnumerator enum_nextPage()
    {
        currentStoryPage++;
        rightStoryPage.GetComponent<Animator>().Play("TurnPageRight");
        rightStoryPage = GameObject.Find("StoryPage_"+currentStoryPage);

        nextButton.SetActive(false);
        backButton.SetActive(false);

        yield return new WaitForSeconds(1);
        string lowerLeftpage = "StoryPage_"+(currentStoryPage-2).ToString();
        Debug.Log(lowerLeftpage);
        foreach(GameObject g in storypageList)
        {
            if(g.name == lowerLeftpage)
            {
                g.SetActive(false);
            }
        }


        yield return new WaitForSeconds(2);
        leftStoryPage = GameObject.Find("StoryPage_"+(currentStoryPage-1).ToString());
 
        nextButton.SetActive(true);
        backButton.SetActive(true);
    }

    public IEnumerator enum_prevPage()
    {
        currentStoryPage--;
        leftStoryPage.GetComponent<Animator>().Play("TurnPageLeft");
        rightStoryPage = GameObject.Find("StoryPage_"+currentStoryPage);

        nextButton.SetActive(false);
        backButton.SetActive(false);

        yield return new WaitForSeconds(1);
        string lowerRightpage = "StoryPage_"+(currentStoryPage-1).ToString();
        Debug.Log(lowerRightpage);
        foreach(GameObject g in storypageList)
        {
            if(g.name == lowerRightpage)
            {
                g.SetActive(true);
            }
        }

        yield return new WaitForSeconds(2);
        leftStoryPage = GameObject.Find("StoryPage_"+(currentStoryPage-1).ToString());


        nextButton.SetActive(true);
        backButton.SetActive(true);

    
        if(currentStoryPage <=0)
        {
            backButton.SetActive(false);
        }
    }

}
