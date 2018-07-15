using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Collections;

public class Chunk
{
    public const int WIDTH =  16; //16
    public const int DEPTH =  16; //16
    public const int HEIGHT = 16;  //16

    public GameObject go;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    int numTris = 0;

    Vector3 noisePosition;

    MarchingCubes.GridCell[] grids;
    MarchingCubes.Triangle[] triangles;
    MarchingCubes mc;
    int triCount;

    public Chunk(GameObject gameObject, Vector3 noiseVector)
    {
        go = gameObject;
        go.transform.parent = GameObject.Find("Terrain").transform;
        meshFilter = go.AddComponent<MeshFilter>();
        meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/Dark Green");
        noisePosition = new Vector3(noiseVector.x, noiseVector.y, noiseVector.z);
        grids = new MarchingCubes.GridCell[WIDTH * HEIGHT * DEPTH];

        mc = new MarchingCubes();
        triangles = new MarchingCubes.Triangle[5];
        triCount = 0;

        //Debug.Log(go.name);

        for (int z = 0; z < DEPTH; z++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    var gridCell = new MarchingCubes.GridCell();
                    gridCell.p[0] = new Vector3(x + 0, y + 0, z + 1);
                    gridCell.p[1] = new Vector3(x + 1, y + 0, z + 1);
                    gridCell.p[2] = new Vector3(x + 1, y + 0, z + 0);
                    gridCell.p[3] = new Vector3(x + 0, y + 0, z + 0);
                    gridCell.p[4] = new Vector3(x + 0, y + 1, z + 1);
                    gridCell.p[5] = new Vector3(x + 1, y + 1, z + 1);
                    gridCell.p[6] = new Vector3(x + 1, y + 1, z + 0);
                    gridCell.p[7] = new Vector3(x + 0, y + 1, z + 0);

                    var noiseIndexX = (int)noisePosition.x + x;
                    var noiseIndexY = (int)noisePosition.y + y;
                    var noiseIndexZ = (int)noisePosition.z + z;

                    #region old 1d array way doesn't quite work right
                    //gridCell.val[0] = MapTerrain.noise[(noiseIndexX + 0) + (noiseIndexY + 0) * MapTerrain.NOISE_WIDTH + (noiseIndexZ + 1) * MapTerrain.NOISE_HEIGHT * MapTerrain.NOISE_WIDTH];
                    //gridCell.val[1] = MapTerrain.noise[(noiseIndexX + 1) + (noiseIndexY + 0) * MapTerrain.NOISE_WIDTH + (noiseIndexZ + 1) * MapTerrain.NOISE_HEIGHT * MapTerrain.NOISE_WIDTH];
                    //gridCell.val[2] = MapTerrain.noise[(noiseIndexX + 1) + (noiseIndexY + 0) * MapTerrain.NOISE_WIDTH + (noiseIndexZ + 0) * MapTerrain.NOISE_HEIGHT * MapTerrain.NOISE_WIDTH];
                    //gridCell.val[3] = MapTerrain.noise[(noiseIndexX + 0) + (noiseIndexY + 0) * MapTerrain.NOISE_WIDTH + (noiseIndexZ + 0) * MapTerrain.NOISE_HEIGHT * MapTerrain.NOISE_WIDTH];
                    //gridCell.val[4] = MapTerrain.noise[(noiseIndexX + 0) + (noiseIndexY + 1) * MapTerrain.NOISE_WIDTH + (noiseIndexZ + 1) * MapTerrain.NOISE_HEIGHT * MapTerrain.NOISE_WIDTH];
                    //gridCell.val[5] = MapTerrain.noise[(noiseIndexX + 1) + (noiseIndexY + 1) * MapTerrain.NOISE_WIDTH + (noiseIndexZ + 1) * MapTerrain.NOISE_HEIGHT * MapTerrain.NOISE_WIDTH];
                    //gridCell.val[6] = MapTerrain.noise[(noiseIndexX + 1) + (noiseIndexY + 1) * MapTerrain.NOISE_WIDTH + (noiseIndexZ + 0) * MapTerrain.NOISE_HEIGHT * MapTerrain.NOISE_WIDTH];
                    //gridCell.val[7] = MapTerrain.noise[(noiseIndexX + 0) + (noiseIndexY + 1) * MapTerrain.NOISE_WIDTH + (noiseIndexZ + 0) * MapTerrain.NOISE_HEIGHT * MapTerrain.NOISE_WIDTH];
                    #endregion

                    gridCell.val[0] = MapTerrain.noise[(noiseIndexX + 0), (noiseIndexY + 0), (noiseIndexZ + 1)];
                    gridCell.val[1] = MapTerrain.noise[(noiseIndexX + 1), (noiseIndexY + 0), (noiseIndexZ + 1)];
                    gridCell.val[2] = MapTerrain.noise[(noiseIndexX + 1), (noiseIndexY + 0), (noiseIndexZ + 0)];
                    gridCell.val[3] = MapTerrain.noise[(noiseIndexX + 0), (noiseIndexY + 0), (noiseIndexZ + 0)];
                    gridCell.val[4] = MapTerrain.noise[(noiseIndexX + 0), (noiseIndexY + 1), (noiseIndexZ + 1)];
                    gridCell.val[5] = MapTerrain.noise[(noiseIndexX + 1), (noiseIndexY + 1), (noiseIndexZ + 1)];
                    gridCell.val[6] = MapTerrain.noise[(noiseIndexX + 1), (noiseIndexY + 1), (noiseIndexZ + 0)];
                    gridCell.val[7] = MapTerrain.noise[(noiseIndexX + 0), (noiseIndexY + 1), (noiseIndexZ + 0)];

                    grids[x + y * WIDTH + z * HEIGHT * WIDTH] = gridCell;

                }
            }
        }
    }
    public void Clear()
    {
        verts.Clear();
        tris.Clear();
        uvs.Clear();
        numTris = 0;
        triCount = 0;
        meshFilter.mesh.Clear();
    }
    public void GenerateTerrain()
    {
        for (int i = 0; i < grids.Length; i++)
        {
            numTris = mc.Polygonize(grids[i], MapTerrain.isolevel, triangles);
            for (int j = 0; j < numTris; j++)
            {
                verts.Add(triangles[j].p[0]);
                verts.Add(triangles[j].p[1]);
                verts.Add(triangles[j].p[2]);

                tris.Add(triCount * 3 + 0);
                tris.Add(triCount * 3 + 1);
                tris.Add(triCount * 3 + 2);

                triCount++;
            }
        }
    }
    public void Render()
    {
        var mesh = meshFilter.mesh;
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }
}