using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class IFCReader
{
    [MenuItem("IFC/ReadIFC")]
    public static void ReadIFC()
    {
        string ifcPath = EditorUtility.OpenFilePanelWithFilters("目标ifc文件", null, new string[] { "Industry Foundation Classes", "ifc" });
        if(string.IsNullOrEmpty(ifcPath)) return;
        WexBimHelper.CreateWexBim(ifcPath);
        return;
        string wexBimPath = ifcPath.Replace(".ifc", ".wexBIM");
        WexBimHelper.ReadWexBim(wexBimPath);

        var projectData= BimHelper.GetBimSpatialStructure(ifcPath);
        ModelCreater.GenerateSpatialStructure(projectData);
        projectData.transform.rotation = Quaternion.Euler(-90, 0, 0);
    }
}
