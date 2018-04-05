using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    float bounds = 50;
    void Start()
    {
        SpawnRandomTrees();
        //GenerateTestTerrain();
    }

    void SpawnRandomTrees()
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

    void GenerateTestTerrain()
    {
        GameObject terrainGO = GameObject.FindGameObjectWithTag("Terrain");
        var terrainData = terrainGO.GetComponent<Terrain>().terrainData;

        var width = terrainData.heightmapWidth;
        var height = terrainData.heightmapHeight;
        float[,] heights = new float[width, height];

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                #region Grassy Mountains
                var octaves = 5;
                var deltaFrequency = 3f;
                var deltaAmplitude = 0.25f;
                var deltaScale = 1.0f;


                var frequency = 0.9f;
                var amplitude = 0.5f;
                var scale = 1.0f;


                for (int i = 0; i < octaves; i++)
                {
                    heights[x,y] += Mathf.PerlinNoise(((float)x / (float)width) * frequency, ((float)y / (float)height) * frequency) * amplitude * scale;
                    frequency *= deltaFrequency;
                    amplitude *= deltaAmplitude;
                    scale *= deltaScale;
                }
                #endregion
            }
        }

        

        terrainData.SetHeights(0, 0, heights);
    }
}
