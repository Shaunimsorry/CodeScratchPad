using UnityEngine;

public class unitAI : MonoBehaviour
{
    void Start()
    {
        MySimpleClass myClass = new MySimpleClass();
        myClass.MyInt = 10;         //set being used
        Debug.Log(myClass.MyInt);   //get being used
        myClass.MyString = "Syper";
        Debug.Log(myClass.MyString);
    }
}


public class MySimpleClass
{
    private int _myInt;
    public int MyInt
    {
        get
        {
            return _myInt;
        }
        set
        {
            _myInt = value;
        }
    }

    public string MyString {get;set;}
}
