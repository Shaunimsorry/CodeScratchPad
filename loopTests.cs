using UnityEngine;

public class unitAI : MonoBehaviour
{
    //Looping tutorial
    //while loop
    public void Start()
    {
        int number = 0;
        //WHILE LOOP
        while (number < 50)
        {
            Debug.Log("Number less than 50");
            number++;
        }

        //DO WHILE LOOP
        do
        {
            Debug.Log("DO LOOP");
            number++;
        }while (number < 50);

        //FOR LOOP
        for (int test1 = 0; test1 < 50; test1++)
        {
            Debug.Log("Pass!");
            //break;            //how to break the statement
        }

        //FOR LOOP ARRAY
        int[] ints = {1,2,3,4,5,6,7,8,9,10};
        for (int i = 0; i<ints.Length; i++)
        {
            ints[i] *= i;
            Debug.Log(ints[i]);
        }

        int[] ints2 = {1,2,3,4,5,6};
        foreach(int myInt in ints2)
        {
            Debug.Log("Shaun");
            Debug.Log(myInt+myInt);
        }
    }
}
