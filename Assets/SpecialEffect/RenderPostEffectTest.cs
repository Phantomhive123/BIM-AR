using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderPostEffectTest : MonoBehaviour
{
    public Material renderMaterial;
    public Shader replaceShader;

    private void LateUpdate()
    {
        if(GetComponent<Camera>() != null && replaceShader != null)
        {
            GetComponent<Camera>().RenderWithShader(replaceShader, "RenderType");
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderMaterial != null)
        {
            Graphics.Blit(source, destination, renderMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
