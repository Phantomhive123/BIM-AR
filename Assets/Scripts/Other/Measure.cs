using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimClick
{
    private GameObject target;
    private int originalLayer;

    private int outLineLayer;

    public AimClick()
    {
        target = null;
        outLineLayer = LayerMask.NameToLayer("OutLine");
    }

    public void Click(RaycastHit raycastHit)
    {
        GameObject obj = raycastHit.collider.gameObject;
        if (obj == target) return;
        if (target != null)
            target.layer = originalLayer;
        originalLayer = obj.layer;
        obj.layer = outLineLayer;
        target = obj;
    }

    public void Reset()
    {
        target.layer = originalLayer;
        target = null;
    }
}

public class DistanceMeasure
{
    private Vector3 pointA;
    private Vector3 pointB;

    private int numCount;
    private LineRenderer lineRenderer;

    public DistanceMeasure()
    {
        GameObject obj = new GameObject("DistanceLine");
        lineRenderer = obj.AddComponent<LineRenderer>();
        Reset();
    }

    public void Click(RaycastHit raycastHit)
    {
        if (raycastHit.collider == null) return;
        Vector3 point = raycastHit.point;

        if (numCount == 0)
        {
            pointA = point;
            lineRenderer.SetPosition(0, pointA);
            numCount++;
        }
        else if (numCount == 1)
        {
            pointB = point;
            lineRenderer.SetPosition(1, pointB);
            numCount++;
            float ans = MathTool.GetDisBetweenPoints(pointA, pointB);
            Debug.Log("Distance is :" + ans);
        }
        else
            Debug.LogWarning("本次测量已经有首尾点");
    }

    public void Reset()
    {
        pointA = pointB = Vector3.zero;
        numCount = 0;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
    }
}

public class AreaMeasure
{
    private GameObject targetPlane;

    public AreaMeasure()
    {
        targetPlane = null;
    }

    public void Click(RaycastHit raycastHit)
    {
        if (raycastHit.collider == null) return;
    }
}
