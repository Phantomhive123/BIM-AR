using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCreater
{
    public static void CreateModel(List<BimProduct> products, List<BimColor> colors)
    {
        foreach(var product in products)
        {
            var productObj = new GameObject(product.entityLabel.ToString());
            var productData = productObj.AddComponent<ProductData>();
            productData.BimProduct = product;
            PublicValue.productsData.Add(productData);
            //忽然感觉position要保留
            //productObj.transform.position = product.position;
            //productObj.transform.localScale = product.scale;
            
            CreateShapes(product, productObj, colors);
        }
    }

    private static void CreateShapes(BimProduct product, GameObject productObj, List<BimColor> colors)
    {
        List<Mesh> meshes = new List<Mesh>();
        List<Material> materials = new List<Material>();

        foreach(var shape in product.shapes)
        {
            Mesh mesh = new Mesh();
            foreach(var tri in shape.triangulations)
            {
                mesh.vertices = tri.vertices.ToArray();
                mesh.triangles = tri.triangles.ToArray();
                mesh.normals = tri.normals.ToArray();
                mesh.Optimize();
            }
            meshes.Add(mesh);

            BimColor bimColor = colors.Find(color => color.styleLabel == shape.styleLabel);
            materials.Add(bimColor.material);
        }

        CombineInstance[] combineInstances = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++) 
        {
            combineInstances[i].mesh = meshes[i];
            combineInstances[i].transform = productObj.transform.localToWorldMatrix;
        }
        
        //创建MeshFilter
        MeshFilter mf = productObj.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combineInstances, false);
        //创建MeshRenderer
        MeshRenderer mr = productObj.AddComponent<MeshRenderer>();
        mr.sharedMaterials = materials.ToArray();
        //创建MeshCollider
        MeshCollider mc = productObj.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;        
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
