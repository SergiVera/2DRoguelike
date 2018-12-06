using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count{
        public int minimum;
        public int maximum;

        public Count(int min, int max){
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    //We add to the list of Vector3's every Vector3, which means from 1 to 7 to let more free space in the map to make easier the level
    void InitialiseList(){
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++){
            for (int y = 1; y < rows - 1; y++){
                gridPositions.Add(new Vector3(x,y,0f));
            }
        }

        ShowToast();
    }

    //We Instantiate the outerWalls and the floor, which goes from -1 (lower left) to the rest extrems
    void BoardSetup(){
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    //We remove a random Vector3, which its index is generated randomly, from the list gridPositions
    Vector3 RandomPosition(){
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    //Here we Instantiate the GameObject and the Vector3 in that order
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum){
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++){
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    //When we start the game, we will follow the following code
    public void SetupScene(int level){
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, wallCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }

    //C# code
    private void ShowToast()
    {
    #if UNITY_ANDROID

        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");

        //create an object array of 3 size
        object[] toastParams = new object[3];

        //create a class reference the unity player activity
        AndroidJavaClass unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        //set the first object in the array as current activity reference
        toastParams[0] = unityActivity.GetStatic<AndroidJavaObject>("currentActivity");

        //set the second object in the array as the CharSequence to be displayed
        toastParams[1] = getGameMap();

        //set the third object in the array as the duration of the toast from
        toastParams[2] = toastClass.GetStatic<int>("LENGTH_LONG");

        AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", toastParams);

        toastObject.Call("show");
    #endif
    }

    private String getGameMap()
    {
#if UNITY_ANDROID
        AndroidJavaObject mapObject = new AndroidJavaObject("MyClass");
        String res = mapObject.Call<String>("getStringMap");
        Debug.Log("String mapa: " + res);
        return res;
#endif
    }

    /*private String dimeHola()
    {
    #if UNITY_ANDROID
        AndroidJavaObject mcObject = new AndroidJavaObject("MiClase");
        String ret = mcObject.Call<String>("miFunc");
        Debug.Log("Palabra: " + ret);
        return ret;
    #else
        return "SOY UN STRING DE PRUEBA";
    #endif
    }*/
}
