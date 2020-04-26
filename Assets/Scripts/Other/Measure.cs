﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.SharedBldgElements;

//鼠标操作类型
public interface IMouseClickType
{
    ClickResult Click(RaycastHit raycastHit);

    void Reset();
}

//用于返回点击信息
public class ClickResult
{
    public string ObjName;
    public string ObjInfo;

    public ClickResult()
    {
        ObjName = "对象无效";
        ObjInfo = "对象无效";
    }
}

public class AimClick:IMouseClickType
{
    private GameObject target;
    private int originalLayer;

    private int outLineLayer;

    public AimClick()
    {
        target = null;
        outLineLayer = LayerMask.NameToLayer("OutLine");
    }

    public ClickResult Click(RaycastHit raycastHit)
    {
        Debug.Log("AimClick!");
        if (raycastHit.collider == null) return null;
        GameObject obj = raycastHit.collider.gameObject;
        ProductData pd = obj.GetComponent<ProductData>();
        if (pd == null) return null;
        if (obj == target) return null;
        if (target != null)
            target.layer = originalLayer;
        originalLayer = obj.layer;
        obj.layer = outLineLayer;
        target = obj;

        ClickResult result = new ClickResult();
        result.ObjName = pd.ProductName;
        result.ObjInfo = pd.IFCProduct.GetType().Name;
        return result;
    }

    public void Reset()
    {
        if (target != null) 
            target.layer = originalLayer;
        target = null;
    }
}

public class DistanceMeasure:IMouseClickType
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

    public ClickResult Click(RaycastHit raycastHit)
    {
        Debug.Log("DistanceClick!");
        if (raycastHit.collider == null) return null;
        Vector3 point = raycastHit.point;

        if (numCount == 0 || numCount >= 2)
        {
            pointA = point;
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, pointA);
            numCount = 1;

            ClickResult result = new ClickResult();
            result.ObjName = "起点";
            result.ObjInfo = point.ToString();
            return result;
        }
        else if (numCount == 1)
        {
            pointB = point;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(1, pointB);
            numCount = 2;
            float ans = MathTool.GetDisBetweenPoints(pointA, pointB);

            ClickResult result = new ClickResult();
            result.ObjName = "两点距离";
            result.ObjInfo = ans.ToString("f3") + "m";
            return result;
        }
        return null;
    }

    public void Reset()
    {
        pointA = pointB = Vector3.zero;
        numCount = 0;
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.02f;
    }
}

public class AreaMeasure:IMouseClickType
{
    private GameObject focusedPlane;
    private List<Type> whiteList;

    public AreaMeasure()
    {
        focusedPlane = new GameObject("TargetPlane");
        focusedPlane.AddComponent<MeshFilter>();
        focusedPlane.AddComponent<MeshRenderer>();
        focusedPlane.layer = LayerMask.NameToLayer("OutLine");

        whiteList = new List<Type>();
        whiteList.Add(typeof(IfcWallStandardCase));
        whiteList.Add(typeof(IfcSlab));
        whiteList.Add(typeof(IfcWall));
    }

    public ClickResult Click(RaycastHit raycastHit)
    {
        Debug.Log("AreaClick!");
        if (raycastHit.collider == null) return null;
        if (raycastHit.collider.GetComponent<MeshCollider>() == null) return null;
        if (raycastHit.collider.GetComponent<MeshFilter>() == null) return null;

        ProductData pd = raycastHit.collider.GetComponent<ProductData>();
        if (pd == null||!whiteList.Contains(pd.IFCProduct.GetType())) return null;

        Mesh mesh = raycastHit.collider.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        List<int> finalVerticeIndex = new List<int>();//最终结果的顶点
        Vector3 aimNormal = normals[triangles[raycastHit.triangleIndex * 3]];//获取目标的相对法线

        //将点击到三角形的三个顶点加入最终结果
        finalVerticeIndex.Add(triangles[raycastHit.triangleIndex * 3]);
        finalVerticeIndex.Add(triangles[raycastHit.triangleIndex * 3 + 1]);
        finalVerticeIndex.Add(triangles[raycastHit.triangleIndex * 3 + 2]);

        //获取法线符合要求的点
        List<int> parallelPoints = new List<int>();
        for (int i = 0; i < normals.Length; i++)
        {
            if (normals[i] == aimNormal)
                parallelPoints.Add(i);
        }

        //获取法线符合要求的三角面
        List<int> parallelTriangles = new List<int>();
        foreach (int i in triangles)
        {
            if (parallelPoints.Contains(i))//之所以可以碰到就加入，是因为unity内置的mesh每个顶点都有三份，只要记录符合法线要求的即可，如果之后出现问题，大概率是在这里
                parallelTriangles.Add(i);
        }

        /*
        MeshInfo meshinfo = focusedPlane.AddComponent<MeshInfo>();
        meshinfo.normals = normals;
        meshinfo.vertices = vertices;
        meshinfo.triangles = triangles;
        meshinfo.aimNormal = aimNormal;
        meshinfo.parallelPoints = parallelPoints.ToArray();
        meshinfo.parallelTriangles = parallelTriangles.ToArray();
        */
    
        //开始获取共面的顶点
        int finalVerticesCount = -1;
        int[] index = new int[3];
        do
        {
            finalVerticesCount = finalVerticeIndex.Count;

            for (int i = 0; i < parallelTriangles.Count;)
            {
                //逐三个读取顶点，获得三角形
                index[0] = parallelTriangles[i++];
                index[1] = parallelTriangles[i++];
                index[2] = parallelTriangles[i++];

                //一旦有一个在最终的面上，三个都在最终的面上
                if (finalVerticeIndex.Contains(index[0]) || finalVerticeIndex.Contains(index[1]) || finalVerticeIndex.Contains(index[2]))
                {
                    if (!finalVerticeIndex.Contains(index[0])) finalVerticeIndex.Add(index[0]);
                    if (!finalVerticeIndex.Contains(index[1])) finalVerticeIndex.Add(index[1]);
                    if (!finalVerticeIndex.Contains(index[2])) finalVerticeIndex.Add(index[2]);
                }
            }
        } while (finalVerticesCount == finalVerticeIndex.Count);//本次循环没有新增点，说明循环结束

        //遍历平行的三角面，获得所有共面的三角面
        List<int> finalTriangles = new List<int>();
        foreach (int i in parallelTriangles)
            if (finalVerticeIndex.Contains(i))
                finalTriangles.Add(i);

        //到这里为止还可以优化，将vertices和normal里面没有用到的都删掉

        Mesh newMesh = new Mesh();
        newMesh.vertices = vertices;
        newMesh.normals = normals;
        newMesh.triangles = finalTriangles.ToArray();

        //这一步要判断三角形面片数是否等于三

        focusedPlane.transform.position = raycastHit.collider.transform.position;
        focusedPlane.transform.rotation = raycastHit.collider.transform.rotation;
        focusedPlane.transform.localScale = raycastHit.collider.transform.localScale;

        focusedPlane.GetComponent<MeshFilter>().mesh = newMesh;
        focusedPlane.GetComponent<MeshRenderer>().material = raycastHit.collider.GetComponent<MeshRenderer>().material;
        focusedPlane.SetActive(true);

        Debug.Log("Area is:" + MathTool.GetTrianglesArea(vertices, finalTriangles.ToArray()));

        ClickResult result = new ClickResult();
        result.ObjName = pd.ProductName;
        string area = MathTool.GetTrianglesArea(vertices, finalTriangles.ToArray()).ToString("f3");
        result.ObjInfo = "面积为：" + area + "平方米";
        return result;
    }


    public void Reset()
    {
        focusedPlane.SetActive(false);
    }
}
