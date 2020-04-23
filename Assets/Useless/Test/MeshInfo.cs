using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInfo : MonoBehaviour
{
    public Vector3[] vertices;
    public Vector3[] normals;
    public int[] triangles;

    public Vector3 aimNormal;
    public int[] parallelPoints;
    public int[] parallelTriangles;
}
