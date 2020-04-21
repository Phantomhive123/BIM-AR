using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc2x3.PresentationResource;

public class Click : MonoBehaviour
{
    Vector3[] vertices;
    Vector3[] normals;
    int[] triangles;
    public List<int> aimPoins;
    public List<int> newTriangles;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var raycastHit);
            if (raycastHit.collider != null)
            {
                Debug.Log(raycastHit.normal);
                Mesh mesh = raycastHit.collider.GetComponent<MeshFilter>().sharedMesh;
                vertices = mesh.vertices;
                normals = mesh.normals;
                triangles = mesh.triangles;
                aimPoins = new List<int>();
                //newTriangles = new List<int>();

                for(int i = 0; i < normals.Length; i++)
                {
                    if (normals[i] == raycastHit.normal) 
                    {
                        aimPoins.Add(i);
                    }
                }
                /*
                foreach(int i in triangles)
                {
                    if(aimPoins.Contains(i))
                    {
                        newTriangles.Add(i);
                    }
                }*/

                //除了这一个之外还要修改vertices和normals数组
                mesh.triangles = newTriangles.ToArray();
            }
        }
    }
}
