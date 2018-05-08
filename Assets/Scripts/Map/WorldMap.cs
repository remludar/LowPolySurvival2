using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    public const int WORLD_WIDTH = 3;
    public const int WORLD_DEPTH = 3;
    public const int CHUNK_WIDTH = 127;
    public const int CHUNK_DEPTH = 127;
    public static float[,] heightMap = new float[WORLD_WIDTH * (CHUNK_WIDTH + 1), WORLD_DEPTH * (CHUNK_DEPTH + 1)];

    List<GameObject> chunks = new List<GameObject>();

    void Start()
    {
        //for (int y = 0; y < WORLD_DEPTH; y++)
        //{
        //    for (int x = 0; x < WORLD_WIDTH; x++)
        //    {
        //        GenerateHeightMap();
        //        var chunkPrefab = Resources.Load("Prefabs/Environmentals/Chunk") as GameObject;
        //        var chunkGO = Instantiate(chunkPrefab, new Vector3(x * CHUNK_WIDTH, 0, y * CHUNK_DEPTH), Quaternion.identity);
        //        chunkGO.transform.parent = transform;
        //        chunks.Add(chunkGO);
        //    }
        //}
        //GenerateHeightMap();
        var pointsList = new List<Vector2>();
        //for(int i =0; i < 100; i++)
        //{
        //    pointsList.Add(new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000)));
        //}
        pointsList.Add(new Vector2(2, 2));
        pointsList.Add(new Vector2(3, 4));
        Voronoi v = new Voronoi(pointsList);
    }

    void GenerateHeightMap()
    {
        #region Coherent Perlin
        var width = WORLD_WIDTH * (CHUNK_WIDTH + 1);
        var depth = WORLD_DEPTH * (CHUNK_DEPTH + 1);

        for (int y = 0; y < depth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var octaves = 3;
                var deltaFrequency = 2.0f;
                var deltaAmplitude = 1.0f;
                var deltaScale = 1.0f;


                var frequency = 0.75f;
                var amplitude = 10.0f;
                var scale = 1.0f;

                for (int i = 0; i < octaves; i++)
                {
                    heightMap[x, y] += Mathf.PerlinNoise(((float)x / (float)width) * frequency, ((float)y / (float)depth) * frequency) * amplitude * scale;
                    frequency *= deltaFrequency;
                    amplitude *= deltaAmplitude;
                    scale *= deltaScale;
                }
            }
        }
        #endregion

        //var width = WORLD_WIDTH * (CHUNK_WIDTH + 1);
        //var depth = WORLD_DEPTH * (CHUNK_DEPTH + 1);

        //for (int y = 0; y < depth; y++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        heightMap[x, y] = 0;
        //    }
        //}
    }
}