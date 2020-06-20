using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTool
{
    public static float GetDisBetweenPoints(Vector3 point_a, Vector3 point_b)
    {
        return Vector3.Distance(point_a, point_b);
    }

    public static float GetTriangleArea(Vector3 pt0, Vector3 pt1, Vector3 pt2)
    {
        float a = (pt1 - pt0).magnitude;
        float b = (pt2 - pt1).magnitude;
        float c = (pt0 - pt2).magnitude;
        float p = (a + b + c) * 0.5f;
        return Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
    }

    public static float GetTrianglesArea(Vector3[] triangles)
    {
        if (triangles.Length % 3 != 0)
            Debug.Log("Triangles array must be times in 3!");

        float result = 0;
        for (int i = 0; i < triangles.Length;)
        {
            result += GetTriangleArea(triangles[i++], triangles[i++], triangles[i++]);
        }
        return result;
    }

    public static float GetTrianglesArea(Vector3[] vertices, int[] triangles)
    {
        Vector3[] tri = new Vector3[triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            tri[i] = vertices[triangles[i]];
        }
        return GetTrianglesArea(tri);
    }
}
