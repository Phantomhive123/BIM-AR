using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        Material[] materials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++) 
        {
            materials[i] = meshRenderers[i].sharedMaterial;
        }
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++) 
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combineInstances, false);
        gameObject.AddComponent<MeshRenderer>().sharedMaterials = materials;

        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        gameObject.AddComponent<MeshCollider>().sharedMesh = mf.mesh;
    }
}
