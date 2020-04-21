using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Scripting.Pipeline;

public class NormalTest : MonoBehaviour
{
    public float length = 1;
    public Vector3 bias;
    public Mesh mesh;
    public Vector3[] vertices;
    public Vector3[] normals;
    public int[] triangls;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        vertices = mesh.vertices;
        normals = mesh.normals;
        triangls = mesh.triangles;
    }
    private void OnDrawGizmos()
    {
        if (mesh == null) return;
        mesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Gizmos.color = Color.red;

        for (int i = 0; i < normals.Length; i++)
        {
            Vector3 pos = vertices[i];
            pos.x *= transform.localScale.x;
            pos.y *= transform.localScale.y;
            pos.z *= transform.localScale.z;

            Gizmos.DrawLine(pos, pos + normals[i] * length);
        }
    }
}
