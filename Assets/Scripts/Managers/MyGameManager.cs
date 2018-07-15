using UnityEngine;
using System.Collections.Generic;

public class MyGameManager : MonoBehaviour
{
    void Start()
    {
        MapTerrain.Generate();
        //GenerateTestForest();
    }

    void Update()
    {
        MapTerrain.Update();
    }

    #region Debug
    void GenerateTestForest()
    {
        var numTrees = 100;

        for (int i = 0; i < numTrees; i++)
        {
            var xLoc = Random.Range(150, 250);
            var zLoc = Random.Range(150, 250);
            var tree2 = Instantiate(Resources.Load<GameObject>("Prefabs/Environmentals/Tree2"), new Vector3(xLoc, 3, zLoc), Quaternion.identity);
            tree2.transform.parent = GameObject.Find("Terrain").transform;
        }

    }
    #endregion

}


