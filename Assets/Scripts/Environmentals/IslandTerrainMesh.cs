using System.Collections.Generic;
using UnityEngine;

public class IslandTerrainMesh : MonoBehaviour
{
    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    const int width = 120;
    const int depth = 120;
    float[,] heightMap = new float[width + 1, depth + 1];

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    int numSquares = 0;

    void Start()
    {
        //GenerateHeightMap();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshCollider = gameObject.GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;
        var uvMat = Resources.Load<Material>("Materials/Dark Green");
        meshRenderer.material = uvMat;

        GenerateHeightMap();
        GenerateMesh();
    }

    void GenerateHeightMap()
    {
        for (int y = 0; y < depth + 1; y++)
        {
            for (int x = 0; x < width + 1; x++)
            {
                var octaves = 16;
                var deltaFrequency = 3f;
                var deltaAmplitude = 0.25f;
                var deltaScale = 1.0f;


                var frequency = 2.9f;
                var amplitude = 10.8f;
                var scale = 2.0f;


                for (int i = 0; i < octaves; i++)
                {
                    heightMap[x, y] += Mathf.PerlinNoise(((float)x / (float)width) * frequency, ((float)y / (float)depth) * frequency) * amplitude * scale;
                    frequency *= deltaFrequency;
                    amplitude *= deltaAmplitude;
                    scale *= deltaScale;
                }
            }
        }
    }
    void GenerateMesh()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                verts.Add(new Vector3(x + 0, heightMap[x + 0, z + 0], z + 0));
                verts.Add(new Vector3(x + 0, heightMap[x + 0, z + 1], z + 1));
                verts.Add(new Vector3(x + 1, heightMap[x + 1, z + 1], z + 1));
                verts.Add(new Vector3(x + 1, heightMap[x + 1, z + 0], z + 0));

                tris.Add(4 * numSquares + 0);
                tris.Add(4 * numSquares + 1);
                tris.Add(4 * numSquares + 2);
                tris.Add(4 * numSquares + 2);
                tris.Add(4 * numSquares + 3);
                tris.Add(4 * numSquares + 0);

                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 1));

                numSquares++;
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }
}