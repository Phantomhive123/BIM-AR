using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc2x3.Interfaces;
using System.Linq;

public class BimHelper
{
    public static ProjectData GetBimSpatialStructure(string ifcFile)
    {
        using (var model = IfcStore.Open(ifcFile))
        {
            //这一整个部分好像都能放进递归里
            var ifcProject = model.Instances.FirstOrDefault<IIfcProject>();
            /*
             * GetSpatialStructure(ifcProject);
             * 好像这一行就可以代替下面所有内容
             */
            var projectObj = new GameObject();
            var projectData = projectObj.AddComponent<ProjectData>();
            projectData.ProjectName = ifcProject.Name;
            projectData.IFCProject = ifcProject;
            //SomeValue.project = pd;
            projectObj.name = projectData.ProjectName;

            //这个循环好像可以放在递归内部，看一下源码中relatedObjects这个类
            foreach (var item in ifcProject.IsDecomposedBy.SelectMany(r => r.RelatedObjects))
            {
                projectData.SubSpatial.Add(GetSpatialStructure(item));
            }
            var spatialProducts = PublicValue.productsData.FindAll(p => p.HaveSpatialStructure == false);
            AddGeoProductsToSpatialStructure(PublicValue.spatialStructures, spatialProducts);
            return projectData;
        }
    }

    private static SpatialData GetSpatialStructure(IIfcObjectDefinition current)
    {
        SpatialData sp = default;
        var spatialElement = current as IIfcSpatialStructureElement;
        if (spatialElement != null) 
        {
            sp = InstantiateCurrentSpatial(spatialElement);
            if (sp != null)
            {
                PublicValue.spatialStructures.Add(sp);
                //考虑一下select需不需要
                //疑问，IIfcSpatialStructureElement的ContainsElements和IIfcObjectDefinition的DecomposedBy是不是一致的
                var containedElements = spatialElement.ContainsElements.SelectMany(e => e.RelatedElements);
                if (containedElements.Count() > 0) 
                {
                    foreach(var element in containedElements)
                    {
                        //需要和public value的productsData生成的地方联系起来看看有没有可以优化的地方
                        var prod = PublicValue.productsData.Find(p => p.BimProduct.entityLabel == element.EntityLabel);
                        if (prod == null)
                        {
                            var go = new GameObject();
                            prod = go.AddComponent<ProductData>();
                            //考虑一下这种情况实际上只用到了BimProduct的Label
                            prod.BimProduct = new BimProduct(element.EntityLabel, (short)element.EntityLabel);
                            //这里和小泽写的不太一样
                            ///prod.SetProductData(element);
                            //sp.SubProducts.Add(prod);
                            prod.SetDecomposedProducts(element.IsDecomposedBy);
                        }
                        prod.SetProductData(element);
                        sp.SubProducts.Add(prod);
                    }
                }
            }
        }
        foreach (var item in current.IsDecomposedBy.SelectMany(r => r.RelatedObjects))
            sp.SubSpatialData.Add(GetSpatialStructure(item));
        return sp;
    }

    private static void AddGeoProductsToSpatialStructure(List<SpatialData> sds, List<ProductData> pds)
    {
        foreach(var sd in sds)
        {
            //这一步是双重循环找一一对应，可能可以优化
            var pd = pds.Find(p => p.BimProduct.entityLabel == sd.IFCStructureElement.EntityLabel);
            if (pd != null)
            {
                pds.Remove(pd);//为了减少范围
                PublicValue.productsData.Remove(pd);
                var spd = sd.gameObject.AddComponent<ProductData>();
                spd.BimProduct = pd.BimProduct;
                spd.IFCProduct = pd.IFCProduct;

                var children = pd.gameObject.GetComponentsInChildren<MeshRenderer>();
                if (sd.IFCStructureElement is IIfcSpace)
                    foreach (var c in children)
                    {
                        c.transform.parent = sd.gameObject.transform;
                        c.gameObject.SetActive(false);
                    }
                else
                    foreach (var c in children)
                        c.transform.parent = sd.gameObject.transform;
                Object.Destroy(pd.gameObject);
            }
        }
    }

    private static SpatialData InstantiateCurrentSpatial(IIfcSpatialStructureElement element)
    {
        var go = new GameObject();
        var sp = go.AddComponent<SpatialData>();
        sp.Name = element.Name;
        sp.IFCStructureElement = element;
        go.name = sp.Name + "[" + sp.IFCStructureElement.GetType().Name + "]#" + sp.IFCStructureElement.EntityLabel;
        PublicValue.spatialStructures.Add(sp);
        return sp;
    }
}
