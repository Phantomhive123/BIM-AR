using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xbim.Ifc2x3.Interfaces;

/// <summary>
/// 存储Project信息
/// </summary>
public class ProjectData : MonoBehaviour
{
    [SerializeField]
    private IIfcProject ifcProject;
    [SerializeField]
    private string projectName;
    [SerializeField]
    private List<ProductData> productsData = new List<ProductData>();
    [SerializeField]
    private List<SpatialData> subSpatial = new List<SpatialData>();

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
