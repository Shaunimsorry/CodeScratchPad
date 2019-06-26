using UnityEngine;

public class unitAI : MonoBehaviour
{
    //array tests
    //single dimensional array

    public void Start()
    {
        //single dimensional array
        int[] ints1 = new int[5];        //another way to do it         
        int[] ints2 = {1,2,3,4,5};       //single dimension array size 5
        ints1[0] = 0;                    //value assinging\
        Debug.Log(ints2.Length);         //check how long an array is
        Debug.Log(ints2[3]);
        //OUTPUT
        //FIRST OUTPUT 5 | SECOND OUTPUT 4

        //jagged arrays
        int[][] jaggedInts = new int[5][];
        jaggedInts[1] = new[] {1,2,3,4,5,6};
        Debug.Log(jaggedInts[1][2]);
        //OUTPUT
        //OUTPUT IS 3 BECAUSE ARRAY [1] IS CALLED TO NUMBER 3 AT POS 2


        //multidimensional arrays
        int[,] multidimensionalArray = {{1,2,3},{1,2,3},{1,2,3}};
        Debug.Log(multidimensionalArray[0,1]);
        //OUTPUT
        //OUTPUT 2 BECAUSE 0 IS FIRST ARRAY AND 1 IS NUMBER AT POS 2
    }
}

