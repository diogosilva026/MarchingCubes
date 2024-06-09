using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

public class MeshGen : MonoBehaviour
{   
    public Material mat;
    MeshRenderer r;
    Mesh m ;

    MeshFilter f;
     
    [ContextMenu("Generate")]
    public void GenerateMesh()
    {
        
        MeshRenderer r = GetComponent<MeshRenderer>();
        if(!r)
        {
           r = gameObject.AddComponent<MeshRenderer>(); 
        }

         f = GetComponent<MeshFilter>();
        if(!f)
        {
           f = gameObject.AddComponent<MeshFilter>(); 
        }

        r.sharedMaterial = mat;

        m = new Mesh();
        f.mesh=m;

        Vector3[] vertices = new Vector3[4];
        
        vertices[0] = new Vector3(-1,1,0);
        vertices[1] = new Vector3(1,1,0);
        vertices[2] = new Vector3(-1,-1,0);
        vertices[3] = new Vector3(1,-1,0);

        //usa os index do array anterior, para simplificar.

        int[] tris  = {0,1,2,1,3,2};

        Vector3[] normals = new Vector3[vertices.Length];
        
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = new Vector3(0,0,-1);
            
        }
        
        Vector2[] uvs = {new(0,1),new(1,1),new(0,0),new(1,0)};
        m.vertices = vertices;
        m.triangles=tris;
        m.normals=normals;
        m.uv= uvs;
        m.RecalculateBounds();
        m.RecalculateNormals();
        
    }
   
   void Start(){
    f = GetComponent<MeshFilter>();
    m = f.sharedMesh;
   }

   void Update(){
     Vector2[] uvs = {new(0,1),new(1,1),new(0,0),new(1,0)};

    Vector2 top = new(Mathf.Sin(Time.time),0);
     Vector2 bottom = new(Mathf.Cos(Time.time),0);

    uvs[0] += top;
    uvs[1] += top;
    uvs[2] += bottom;
    uvs[3] += bottom;

    m.uv=uvs;
    
   }
}
