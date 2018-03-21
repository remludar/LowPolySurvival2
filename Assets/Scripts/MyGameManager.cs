using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    float bounds = 50;
    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        Random.State seedGenerator = Random.state;

        var terrainGO = GameObject.Find("Terrain");
        for (int i = 0; i < 50; i++)
        {
            var rotateX = Random.Range(0.0f, 360.0f);
            var rotateZ = Random.Range(0.0f, 360.0f);
            string treeToSpawn = (Random.Range(0, 2) == 0) ? "Tree" : "Tree2";
            var rotation = Quaternion.LookRotation(new Vector3(rotateX, 0, rotateZ));
            var treeGO = Instantiate(Resources.Load("Prefabs/" + treeToSpawn), new Vector3(Random.Range(-bounds, bounds), 0, Random.Range(-bounds, bounds)), rotation) as GameObject;
            var size = Random.Range(0.0f, 2.0f);
            treeGO.transform.localScale += new Vector3(size, size, size);
            treeGO.name = System.Guid.NewGuid().ToString();
            treeGO.transform.parent = terrainGO.transform;
            
        }
    }
}