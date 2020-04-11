using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //平移旋转进退
    private Vector3 originalPos = Vector3.zero;
    private Vector3 originalRotation = Vector3.zero;

    private Vector3 mousePosLastFrame = Vector3.zero;
    private Vector3 mousePosCurrentFrame = Vector3.zero;

    [SerializeField]
    private float HorizontalScaler = 0.01f;//这个系数也需要
    [SerializeField]
    private float VerticalScaler = 1f;//感觉这个系数需要动态变化
    [SerializeField]
    private float RotationScaler = 1;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        originalRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {/*
        if(Input.GetMouseButtonDown(2))
        {
            mousePosLastFrame = Input.mousePosition;
        }
        if(Input.GetMouseButton(2))
        {
            mousePosCurrentFrame = Input.mousePosition;
            transform.position -= (mousePosCurrentFrame - mousePosLastFrame) * HorizontalScaler;
            mousePosLastFrame = mousePosCurrentFrame;
        }
        */
        if (Input.GetMouseButtonDown(1))
        {
            mousePosLastFrame = Input.mousePosition;
        }
        if(Input.GetMouseButton(1))
        {
            mousePosCurrentFrame = Input.mousePosition;
            Vector3 delta = mousePosCurrentFrame - mousePosLastFrame;
            Vector3 rotation = new Vector3(delta.y, -delta.x, 0);
            //Debug.Log()
            transform.Rotate(rotation * RotationScaler, Space.Self);
            mousePosLastFrame = mousePosCurrentFrame;
        }

        float scrollWheelMovement = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scrollWheelMovement * VerticalScaler;
        
    }

    private void FixedUpdate()
    {
        
    }
}
