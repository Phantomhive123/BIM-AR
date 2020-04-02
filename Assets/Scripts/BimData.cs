using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Common.Geometry;
using Xbim.Ifc;

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
    public int styleLabel;
    public float r, g, b, a;
    public Material material;
    public BimColor(int Label, float R, float G, float B, float A = 1.0f)
    {
        styleLabel = Label;
        r = R; g = G; b = B; a = A;

        material = new Material(Shader.Find("Standard"));
        if (a != 1.0f) 
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
        material.color = new Color(r, g, b, a);
    }
}

public struct BimProduct
{
    public int entityLabel;
    public short productType;
    public Vector3 position;    //好像可以删掉
    public Vector3 scale;       //好像可以删掉
    public List<BimShape> shapes;

    public BimProduct(int Label,short ID)
    {
        entityLabel = Label;
        productType = ID;
        position = Vector3.zero;
        scale = Vector3.zero;
        shapes = new List<BimShape>();
    }

    public BimProduct(int Label, short ID, Vector3 Pos, Vector3 Scale)
    {
        entityLabel = Label;
        productType = ID;
        position = Pos;
        scale = Scale;
        shapes = new List<BimShape>();
    }

    public BimProduct(int Label, short Type, float px, float py, float pz, float sx, float sy, float sz)
    {
        entityLabel = Label;
        productType = Type;
        position = new Vector3(px, py, pz);
        scale = new Vector3(sx, sy, sz);
        shapes = new List<BimShape>();
    }
}

public struct BimShape
{
    public int ifcProductLabel;
    public short ifcTypeID;
    public int instanceLabel;
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

        bimTriangulation.ToPointsWithNormalsAndIndices(out List<float[]> points, out List<int> indices);
        triangles = indices;
        foreach (var p in points)
        {
            var vertice = new Vector3(p[0], p[1], p[2]) / scale - offset;
            var normal = new Vector3(p[3], p[4], p[5]);
            vertices.Add(vertice);
            normals.Add(normal);
        }

        /*
        foreach (var v in bimTriangulation.Vertices)
            vertices.Add(new Vector3((float)v.X, (float)v.Y, (float)v.Z) / scale - offset);
        foreach(var f in bimTriangulation.Faces)
        {
            triangles.AddRange(f.Indices);
            foreach(var n in f.Normals)
            {
                normals.Add(new Vector3((float)n.Normal.X, (float)n.Normal.Y, (float)n.Normal.Z));
            }
        }
        */
    }
}