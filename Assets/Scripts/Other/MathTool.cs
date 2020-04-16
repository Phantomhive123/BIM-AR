﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTool
{

    public static float GetDisBetweenPoints(Vector3 point_a, Vector3 point_b)
    {
        return Vector3.Distance(point_a, point_b);
    }

    public static float GetArea(RaycastHit raycastHit)
    {
        if (raycastHit.collider != null)
        {
            GameObject obj = raycastHit.collider.gameObject;
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            Mesh m = meshFilter.mesh;
        }
        return 0;
    }
}
