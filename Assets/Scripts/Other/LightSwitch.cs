using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    
    public void SetLightActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
