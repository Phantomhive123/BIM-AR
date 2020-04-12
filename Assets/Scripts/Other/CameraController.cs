﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //平移旋转进退
    private Vector3 originalPos = Vector3.zero;
    private Quaternion originalRotation = Quaternion.identity;

    private Vector3 mousePosLastFrame = Vector3.zero;
    private Vector3 mousePosCurrentFrame = Vector3.zero;

    [SerializeField]
    private float HorizontalScaler = 0.01f;//这个系数也需要
    [SerializeField]
    private float VerticalScaler = 1f;//感觉这个系数需要动态变化
    [SerializeField]
    private float RotationScaler = 1f;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {
            mousePosLastFrame = Input.mousePosition;
        }
        if(Input.GetMouseButton(2))
        {
            mousePosCurrentFrame = Input.mousePosition;
            transform.Translate((mousePosLastFrame - mousePosCurrentFrame) * HorizontalScaler);
            mousePosLastFrame = mousePosCurrentFrame;
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            mousePosLastFrame = Input.mousePosition;
        }
        if(Input.GetMouseButton(1))
        {
            mousePosCurrentFrame = Input.mousePosition;
            Vector3 delta = mousePosCurrentFrame - mousePosLastFrame;
            delta *= RotationScaler;
            transform.Rotate(Vector3.up, delta.x, Space.World);
            transform.Rotate(Vector3.left, delta.y);
            mousePosLastFrame = mousePosCurrentFrame;
        }

        float scrollWheelMovement = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scrollWheelMovement * VerticalScaler;
        
    }

    private void FixedUpdate()
    {
        
    }

    public void ResetCamera()
    {
        transform.SetPositionAndRotation(originalPos, originalRotation);
    }
}
