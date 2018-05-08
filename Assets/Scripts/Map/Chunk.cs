using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    int numSquares = 0;

    void Start()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        mesh = meshFilter.mesh;
        var uvMat = Resources.Load<Material>("Materials/Dark Green");
        meshRenderer.material = uvMat;

        GenerateMesh();
    }


    public void GenerateMesh()
    {
        mesh.Clear();
        verts.Clear();
        tris.Clear();
        uvs.Clear();
        numSquares = 0;

        for (int z = 0; z < WorldMap.CHUNK_DEPTH; z++)
        {
            for (int x = 0; x < WorldMap.CHUNK_WIDTH; x++)
            {
                int mapX = (int)transform.position.x + x;
                int mapZ = (int)transform.position.z + z;

                verts.Add(new Vector3(x + 0, WorldMap.heightMap[mapX + 0, mapZ + 0], z + 0));
                verts.Add(new Vector3(x + 0, WorldMap.heightMap[mapX + 0, mapZ + 1], z + 1));
                verts.Add(new Vector3(x + 1, WorldMap.heightMap[mapX + 1, mapZ + 1], z + 1));
                verts.Add(new Vector3(x + 1, WorldMap.heightMap[mapX + 1, mapZ + 0], z + 0));

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