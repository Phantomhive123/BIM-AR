using System.IO;
using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using System.Collections.Generic;

public class WexBimHelper
{
    private const string EXE_PATH= "D:\\Desktop\\wexBimHelper\\wexHelper.exe";
    private static string IFC_PATH = null;

    [MenuItem("IFC/CreateWexBim")]
    public static void CreateWexBim()
    {
        var tem = EditorUtility.OpenFilePanelWithFilters("目标ifc文件", null, new string[] { "Industry Foundation Classes", "ifc" });
        if (string.IsNullOrEmpty(tem)) return;
        IFC_PATH = tem;
        try
        {
            Process myprocess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(EXE_PATH, tem);
            myprocess.StartInfo = startInfo;
            myprocess.StartInfo.UseShellExecute = false;
            myprocess.Start();
            myprocess.WaitForExit();
            UnityEngine.Debug.Log("Done");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("出错原因：" + ex.Message);
        } 
    }

    [MenuItem("IFC/CreateModelFromWexBim")]
    public static void ReadWexBim()
    {
        Dictionary<int, BimColor> colors = new Dictionary<int, BimColor>();
        List<BimRegion> regions = new List<BimRegion>();
        List<BimProduct> products = new List<BimProduct>();

        var tem = EditorUtility.OpenFilePanelWithFilters("目标wexBim文件", null, new string[] { "wexBim", "wexBIM" });
        using (var fs = new FileStream(tem, FileMode.Open, FileAccess.Read))
        {
            float scale = 0f;
            Vector3 offset = Vector3.zero;
            using (var br = new BinaryReader(fs))
            {
                var magicNumber = br.ReadInt32();
                var version = br.ReadByte();
                var shapeCount = br.ReadInt32();
                var vertexCount = br.ReadInt32();
                var triangleCount = br.ReadInt32();
                var matrixCount = br.ReadInt32();
                var productCount = br.ReadInt32();
                var styleCount = br.ReadInt32();
                var meter = br.ReadSingle();
                var regionCount = br.ReadInt16();
                scale = meter;

                for (int i = 0; i < regionCount; i++)
                {
                    var population = br.ReadInt32();
                    var centreX = br.ReadSingle();
                    var centreY = br.ReadSingle();
                    var centreZ = br.ReadSingle();
                    var boundsBytes = br.ReadBytes(6 * sizeof(float));
                    var modelBounds = XbimRect3D.FromArray(boundsBytes);

                    BimRegion region = new BimRegion(population, centreX, centreY, centreZ, (float)modelBounds.SizeX, (float)modelBounds.SizeY, (float)modelBounds.SizeZ);
                    regions.Add(region);
                }

                offset = regions[0].position;
                
                for (int i = 0; i < styleCount; i++)
                {
                    var styleId = br.ReadInt32();
                    var red = br.ReadSingle();
                    var green = br.ReadSingle();
                    var blue = br.ReadSingle();
                    var alpha = br.ReadSingle();

                    BimColor color = new BimColor(styleId, red, green, blue, alpha);
                    colors.Add(color.styleLabel, color);
                }

                for (int i = 0; i < productCount; i++)
                {
                    var productLabel = br.ReadInt32();
                    var productType = br.ReadInt16();
                    var boxBytes = br.ReadBytes(6 * sizeof(float));
                    XbimRect3D bb = XbimRect3D.FromArray(boxBytes);

                    XbimPoint3D doubleCenter = XbimPoint3D.Add(bb.Min, bb.Max);
                    BimProduct product = new BimProduct(productLabel, productType, (float)doubleCenter.X / 2, (float)doubleCenter.Y / 2,
                        (float)doubleCenter.Z / 2, (float)bb.SizeX, (float)bb.SizeY, (float)bb.SizeZ);
                    products.Add(product);               
                }

                for (int i = 0; i < shapeCount; i++)
                {
                    var shapeRepetition = br.ReadInt32();
                    if (shapeRepetition > 1)
                    {
                        List<BimShape> thisShape = new List<BimShape>();
                        for (int j = 0; j < shapeRepetition; j++)
                        {
                            var ifcProductLabel = br.ReadInt32();
                            var instanceTypeId = br.ReadInt16();
                            var instanceLabel = br.ReadInt32();
                            var styleId = br.ReadInt32();
                            var transform = XbimMatrix3D.FromArray(br.ReadBytes(sizeof(double) * 16));

                            BimShape shape = new BimShape(ifcProductLabel, instanceTypeId, instanceLabel, styleId, transform);
                            thisShape.Add(shape);
                            var p = products.Find(product => product.entityLabel == ifcProductLabel);
                            p.shapes.Add(shape);
                        }
                        var triangulation = br.ReadShapeTriangulation();
                        foreach(var s in thisShape)
                        {
                            var tri = new BimTriangulation(triangulation, scale, offset, s.transform, true);
                            s.triangulations.Add(tri);
                        }
                    }
                    else if (shapeRepetition == 1)
                    {
                        var ifcProductLabel = br.ReadInt32();
                        var instanceTypeId = br.ReadInt16();
                        var instanceLabel = br.ReadInt32();
                        var styleId = br.ReadInt32();
                        XbimShapeTriangulation triangulation = br.ReadShapeTriangulation();
                        
                        BimShape shape = new BimShape(ifcProductLabel, instanceTypeId, instanceLabel, styleId);
                        var p = products.Find(product => product.entityLabel == ifcProductLabel);
                        p.shapes.Add(shape);
                        var tri = new BimTriangulation(triangulation, scale, offset);
                        shape.triangulations.Add(tri);
                    }
                }
            }
        }
        ModelCreater.CreateModel(products, colors);

        if(tem.EndsWith(".wexBIM"))
        {
            var ifcPath = tem.Replace(".wexBIM", ".ifc");
            var projectData = BimHelper.GetBimSpatialStructure(ifcPath);
            ModelCreater.GenerateSpatialStructure(projectData);
            projectData.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }
    }

    
}
