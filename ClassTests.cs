using UnityEngine;

public class unitAI : MonoBehaviour
{
    MySimpleClass shaun = new MySimpleClass();
    void Start()
    {
    }
    void Update()
    {
        shaun.counter = (int)(Mathf.Round(Time.timeSinceLevelLoad));
        shaun.setMyage();
        Debug.Log(shaun.counter);
        Debug.Log(shaun.myAge);
    }
}


public class MySimpleClass
{
    public int counter{get;set;}
    public int myAge {get;private set;}

    public void setMyage()
    {
        if(counter <= 5)
        {
            myAge = 10;
        }
        if(counter >= 5)
        {
            myAge = 100;
            counter = 1;            //Reset
        }
    }
}
