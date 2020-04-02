using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCreater
{
    public static void CreateModel(List<BimProduct> products, Dictionary<int, BimColor> colors)
    {
        foreach(var product in products)
        {
            var productObj = new GameObject(product.entityLabel.ToString());//product的name需要想办法
            var productData = productObj.AddComponent<ProductData>();
            productData.BimProduct = product;
            PublicValue.productsData.Add(productData);
            productObj.transform.position = product.position;
            productObj.transform.localScale = product.scale;
            
            CreateShape(product, productObj, colors);
        }
    }

    private static void CreateShape(BimProduct product, GameObject productObj, Dictionary<int, BimColor> colors)
    {
        foreach (var shape in product.shapes)
        {
            GameObject shapeObj = new GameObject(shape.instanceLabel.ToString());//这个name又该怎么办呢
            shapeObj.transform.parent = productObj.transform;

            var meshFilter = shapeObj.AddComponent<MeshFilter>();
            var mesh = meshFilter.mesh;
            //Debug.Log(productObj.name + ":" + shapeObj.name + ":" + shape.triangulations.Count);
            foreach(var tri in shape.triangulations)//?
            {
                mesh.vertices = tri.vertices.ToArray();
                mesh.triangles = tri.triangles.ToArray();
                mesh.normals = tri.normals.ToArray();
                mesh.Optimize();
                //mesh.RecalculateNormals();
            }

            var meshRenderer = shapeObj.AddComponent<MeshRenderer>();          
            BimColor bimColor;
            if(colors.TryGetValue(shape.styleLabel, out bimColor))        
                meshRenderer.material = bimColor.material;
            //shapeObj.AddComponent<MeshCollider>();
        }
    }

    public static void GenerateSpatialStructure(ProjectData projectData)
    {
        foreach(var spatial in projectData.SubSpatial)
        {
            spatial.gameObject.transform.parent = projectData.gameObject.transform;
            FindSubSpatialStructure(spatial);
        }
    }

    private static void FindSubSpatialStructure(SpatialData spatial)
    {
        HashSet<string> typeName = new HashSet<string>();
        foreach(var p in spatial.SubProducts)
        {
            if(!typeName.Contains(p.TypeName))
            {
                typeName.Add(p.TypeName);
                var gameObj = new GameObject(p.TypeName);
                gameObj.transform.parent = spatial.transform;
            }
            p.transform.parent = spatial.transform.Find(p.TypeName);
            if (p.DecomposedProducts.Count > 0)
            {
                foreach (var dp in p.DecomposedProducts)
                    dp.transform.parent = p.transform;
            }
        }
        foreach(var ss in spatial.SubSpatialData)
        {
            ss.transform.parent = spatial.transform;
            FindSubSpatialStructure(ss);
        }
    }
}
