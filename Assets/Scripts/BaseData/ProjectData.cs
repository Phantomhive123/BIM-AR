using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc2x3.Interfaces;

public class ProjectData : MonoBehaviour
{
    [SerializeField]
    private IIfcProject ifcProject;//判断是否需要
    [SerializeField]
    private string projectName;
    [SerializeField]
    private List<ProductData> productsData = new List<ProductData>();//判断是否需要
    [SerializeField]
    private List<SpatialData> subSpatial = new List<SpatialData>();//判断是否需要


    public IIfcProject IFCProject
    {
        get { return ifcProject; }
        set { ifcProject = value; }
    }

    public string ProjectName
    {
        get { return projectName; }
        set { projectName = value; }
    }

    public List<ProductData> ProductsData
    {
        get { return productsData; }
        set { productsData = value; }
    }

    public List<SpatialData> SubSpatial
    {
        get { return subSpatial; }
        set { subSpatial = value; }
    }
}
