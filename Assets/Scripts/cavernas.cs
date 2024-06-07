using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CaveMaker3D : MonoBehaviour
{
    public int width, height, depth;
    public static bool[,,] map;
    public int seed = 0;
    public int iterations;
    public int threshold;

    [Range(0f, 1f)]
    public float fillPercentage;

    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateMap();
    }

    [ContextMenu("Generate New Map")]
    public void GenerateMap()
    {
        if (!meshFilter)
            meshFilter = GetComponent<MeshFilter>();

        map = new bool[width, height, depth];
        GenerateNoise();
        for (int i = 0; i < iterations; i++)
           Iterate();
       // UpdateMesh();

        for (int z = 0; z < depth; z++)
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (map[x, y, z] == true) 
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.gameObject.tag = "cube";
                        cube.transform.position = new Vector3(x, y, z);
                        cube.gameObject.GetComponent<MeshRenderer>().sharedMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
                    }
                }
    }

    [ContextMenu("Delete All Cubes")]

    public void DeleteAllCubes()
    {
            var goArray = GameObject.FindGameObjectsWithTag("cube");
            foreach (var item in goArray)
            {
                DestroyImmediate(item);
            }
    }

    private void GenerateNoise()
    {
        UnityEngine.Random.InitState(seed != 0 ? seed : (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
        int count=0;

        for (int z = 0; z < depth; z++)
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    map[x, y, z] = (UnityEngine.Random.value < fillPercentage);
                    //if (x == 0 || x == width - 1 || y == 0 || y == height - 1 || z == 0 || z == depth - 1)
                    //{
                    //    map[x, y, z] = true;
                    //}
                    //else
                    //{
                    //    Debug.Log("dwd");
                    //}
                    if (map[x, y, z] == true)
                    {

                        count++;
                    }
                }
        Debug.Log(count);
    }   

    private void Iterate()
    {
        if (map == null)
            GenerateMap();

        bool[,,] newMap = new bool[width, height, depth];

        for (int z = 0; z < depth; z++)
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int neighbours = CountNeighbours(x, y, z);

                    newMap[x, y, z] = neighbours < threshold ? false : neighbours > threshold ? true : map[x, y, z];
                }

        map = newMap;
    }

    private int CountNeighbours(int xPos, int yPos, int zPos)
    {
        int count = 0;

        for (int z = zPos - 1; z <= zPos + 1; z++)
            for (int y = yPos - 1; y <= yPos + 1; y++)
                for (int x = xPos - 1; x <= xPos + 1; x++)
                    if (x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth)
                    {
                        if (x != xPos || y != yPos || z != zPos)
                            count += map[x, y, z] ? 1 : 0;
                    }
                    else
                        count++;

        return count;
    }

   //private void UpdateMesh()
   // {
   //     List<Vector3> vertices = new List<Vector3>();
   //     List<int> triangles = new List<int>();
   //     List<Vector2> uvs = new List<Vector2>();

   //     for (int z = 0; z < depth; z++)
   //         for (int y = 0; y < height; y++)
   //             for (int x = 0; x < width; x++)
   //             {
   //                 if (map[x, y, z])
   //                 {
   //                     Vector3 pos = new Vector3(x, y, z);
   //                     AddCube(vertices, triangles, uvs, pos);
   //                 }
   //             }

   //     Mesh mesh = new Mesh();
   //     mesh.vertices = vertices.ToArray();
   //     mesh.triangles = triangles.ToArray();
   //     mesh.uv = uvs.ToArray();
   //     mesh.RecalculateNormals();

   //     meshFilter.mesh = mesh;
   // } 

    private void AddCube(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, Vector3 pos)
    {
        int vertexIndex = vertices.Count;

        Vector3[] cubeVertices = {
            pos + new Vector3(0, 0, 0),
            pos + new Vector3(1, 0, 0),
            pos + new Vector3(1, 1, 0),
            pos + new Vector3(0, 1, 0),
            pos + new Vector3(0, 1, 1),
            pos + new Vector3(1, 1, 1),
            pos + new Vector3(1, 0, 1),
            pos + new Vector3(0, 0, 1)
        };

        vertices.AddRange(cubeVertices);


        int[] cubeTriangles = {
            0, 2, 1, 0, 3, 2, // Front
            2, 3, 4, 2, 4, 5, // Top
            1, 2, 5, 1, 5, 6, // Right
            0, 7, 4, 0, 4, 3, // Left
            5, 4, 7, 5, 7, 6, // Back
            0, 6, 7, 0, 1, 6  // Bottom
        };

        for (int i = 0; i < cubeTriangles.Length; i++)
        {
            triangles.Add(vertexIndex + cubeTriangles[i]);
        }


        Vector2[] cubeUVs = {
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Front
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0)  // Back
        };

        uvs.AddRange(cubeUVs);
    }
}
