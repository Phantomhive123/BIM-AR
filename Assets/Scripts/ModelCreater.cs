using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCreater
{
    public static void CreateModel(List<BimProduct> products, Dictionary<int, BimColor> colors)
    {
        GameObject mainObj = new GameObject();//物体名字可以命名为文件名
        List<GameObject> productObjs = new List<GameObject>();
        foreach(var product in products)
        {
            var productObj = new GameObject(product.productLabel.ToString());//product的name需要想办法
            productObj.transform.position = product.position;
            productObj.transform.localScale = product.scale;
            productObj.transform.parent = mainObj.transform;

            productObjs.Add(productObj);
            CreateShape(product, productObj, colors);
        }
        mainObj.transform.rotation = Quaternion.Euler(-90, 0, 0);
    }

    private static void CreateShape(BimProduct product, GameObject productObj, Dictionary<int, BimColor> colors)
    {
        foreach (var shape in product.shapes)
        {
            GameObject shapeObj = new GameObject(shape.instanceLabel.ToString());//这个name又该怎么办呢
            shapeObj.transform.parent = productObj.transform;

            var meshFilter = shapeObj.AddComponent<MeshFilter>();
            var mesh = meshFilter.mesh;
            Debug.Log(productObj.name + ":" + shapeObj.name + ":" + shape.triangulations.Count);
            foreach(var tri in shape.triangulations)//?
            {
                mesh.vertices = tri.vertices.ToArray();
                mesh.triangles = tri.triangles.ToArray();
                mesh.Optimize();
                mesh.RecalculateNormals();
            }

            var meshRenderer = shapeObj.AddComponent<MeshRenderer>();          
            BimColor bimColor;
            if(colors.TryGetValue(shape.styleLabel, out bimColor))        
                meshRenderer.material = bimColor.material;            
        }
    }

}
