using UnityEngine;

public class MyGameManager : MonoBehaviour
{

    void Start()
    {
        //GenerateTestTerrain();
        //IslandTerrainMesh.Generate();
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
                //var octaves = 16;
                //var deltaFrequency = 0.5f;
                //var deltaAmplitude = 0.25f;
                //var deltaScale = 1.0f;


                //var frequency = 0.9f;
                //var amplitude = 0.8f;
                //var scale = (1024.0f / (width * height)) * 0.0033f;

                var octaves = 16;
                var deltaFrequency = 3f;
                var deltaAmplitude = 0.25f;
                var deltaScale = 1.0f;


                var frequency = 0.9f;
                var amplitude = 0.8f;
                var scale = 2.0f;


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
