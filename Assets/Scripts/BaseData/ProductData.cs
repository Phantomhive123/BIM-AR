using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc2x3.Interfaces;

public class ProductData : MonoBehaviour
{
    [SerializeField]
    private BimProduct bimProduct;
    [SerializeField]
    private string typeName;
    [SerializeField]
    private string productName;
    [SerializeField]
    private bool haveSpatialStructure = false;
    [SerializeField]
    private IIfcProduct ifcProduct;//目前看来用不到，如果用不到需要修改set方法的变量名
    [SerializeField]
    private List<ProductData> decomposedProducts = new List<ProductData>();

    public BimProduct BimProduct
    {
        get { return bimProduct; }
        set { bimProduct = value; }
    }

    public string TypeName
    {
        get { return typeName; }
        set { typeName = value; }
    }

    public string ProductName
    {
        get { return productName; }
        set { productName = value; }
    }

    public bool HaveSpatialStructure
    {
        get { return haveSpatialStructure; }
        set { haveSpatialStructure = value; }
    }

    public IIfcProduct IFCProduct
    {
        get { return ifcProduct; }
        set { ifcProduct = value; }
    }

    public List<ProductData> DecomposedProducts
    {
        get { return decomposedProducts; }
        set { decomposedProducts = value; }
    }

    public void SetProductData(IIfcProduct iProduct)
    {
        ProductName = iProduct.Name;
        TypeName = iProduct.GetType().Name;
        //ThisGameObject.name = pd.ProductName + "[" + pd.TypeName + "]#" + pd.ProductGeoData.entityLabel;
        gameObject.name = ProductName + "[" + TypeName + "]#" + bimProduct.entityLabel;
        IFCProduct = iProduct;//看需要判断删不删掉
        HaveSpatialStructure = true;
    }

    public void SetDecomposedProducts(IEnumerable<IIfcRelDecomposes> connects)
    {
        List<ProductData> pds = new List<ProductData>();
        foreach(var c in connects)
        {
            foreach(var relatedObj in c.RelatedObjects)
            {
                var pd = PublicValue.productsData.Find(p => p.BimProduct.entityLabel == relatedObj.EntityLabel);
                pd.SetProductData((IIfcProduct)relatedObj);
                pds.Add(pd);
            }
        }
        decomposedProducts = pds;
    }
}
