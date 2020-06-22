using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Common.Geometry;

public struct BimRegion
{
    public int population;
    public Vector3 position;
    public Vector3 scale;

    public BimRegion(int Population,float px, float py, float pz, float sx, float sy, float sz)
    {
        population = Population;
        position = new Vector3(px, py, pz);
        scale = new Vector3(sx, sy, sz);
    }
}

public struct BimColor
{
    //材质的索引
    public int styleLabel;
    //材质的引用
    public Material material;
    public BimColor(int Label, float R, float G, float B, float A = 1.0f)
    {
        styleLabel = Label;

        material = new Material(Shader.Find("Standard"));
        if (A != 1.0f) 
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
        material.color = new Color(R, G, B, A);
    }
}

public struct BimProduct
{
    //构件模型索引
    public int entityLabel;
    //构件类型
    public short productType;
    //构件子形体组
    public List<BimShape> shapes;

    public BimProduct(int Label,short ID)
    {
        entityLabel = Label;
        productType = ID;
        shapes = new List<BimShape>();
    }
}

public struct BimShape
{
    //所属构件的索引，用于形体和构件绑定
    public int ifcProductLabel;
    public short ifcTypeID;
    public int instanceLabel;
    //材质索引
    public int styleLabel;
    public List<BimTriangulation> triangulations;
    public XbimMatrix3D transform;
    public BimShape(int ProductLabel, short TypeID, int InstanceLabel, int StyleLabel, XbimMatrix3D Matrix = new XbimMatrix3D())
    {
        ifcProductLabel = ProductLabel;
        ifcTypeID = TypeID;
        instanceLabel = InstanceLabel;
        styleLabel = StyleLabel;
        triangulations = new List<BimTriangulation>();
        transform = Matrix;
    }
}

public struct BimTriangulation
{
    //xBim中三角面片数据
    public XbimShapeTriangulation bimTriangulation;
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector3> normals;
    public BimTriangulation(XbimShapeTriangulation triangulation, float scale, Vector3 offset, XbimMatrix3D matrix = new XbimMatrix3D(), bool bMatrix = false)
    {
        if (bMatrix)
        {
            bimTriangulation = triangulation.Transform(matrix);
        }
        else
            bimTriangulation = triangulation;

        vertices = new List<Vector3>();       
        normals = new List<Vector3>();

        bimTriangulation.ToPointsWithNormalsAndIndices(out List<float[]> positions, out List<int> indices);
        triangles = indices;
        foreach (var p in positions)
        {
            //原版减去了一个offset，但是我发现不计算offset所有偏移都是正确的
            var vertice = new Vector3(p[0], p[1], p[2]) / scale - offset;
            //var vertice = new Vector3(p[0], p[1], p[2]) / scale;
            var normal = new Vector3(p[3], p[4], p[5]);
            vertices.Add(vertice);
            normals.Add(normal);
        }
    }
}