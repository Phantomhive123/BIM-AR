using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc4.Interfaces;

public class SpatialData : MonoBehaviour
{
    [SerializeField]
    private IIfcSpatialStructureElement ifcStructureElement;//这个包括了entityLabel，后续看看是不是存个entityLabel更合适
    [SerializeField]
    private string elementName;
    [SerializeField]
    private List<ProductData> subProducts = new List<ProductData>();
    [SerializeField]
    private List<SpatialData> subSpatialData = new List<SpatialData>();

    public IIfcSpatialStructureElement IFCStructureElement
    {
        get { return ifcStructureElement;}
        set { ifcStructureElement = value; }
    }
    public string Name
    {
        get { return elementName; }
        set { elementName = value; }
    }
    public List<ProductData> SubProducts
    {
        get { return subProducts; }
        set { subProducts = value; }
    }
    public List<SpatialData> SubSpatialData
    {
        get { return subSpatialData; }
        set { subSpatialData = value; }
    }
}
