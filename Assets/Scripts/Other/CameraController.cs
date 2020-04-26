using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CameraMode
{
    AimClick,
    DistanceMeasure,
    AreaMeasure
}

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CameraMode currentCameraMode;
    private IMouseClickType currentClickType;
    private AimClick aimClick;
    private DistanceMeasure distanceMeasure;
    private AreaMeasure areaMeasure;

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

    [SerializeField]
    private Text nameLabel;
    [SerializeField]
    private Text infoLabel;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        originalRotation = transform.rotation;
        aimClick = new AimClick();
        distanceMeasure = new DistanceMeasure();
        areaMeasure = new AreaMeasure();

        SetCameraMode(CameraMode.AimClick);
    }

    // Update is called once per frame
    void Update()
    {    
        /*
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (Input.GetMouseButtonDown(0))
            {
                mousePosLastFrame = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                mousePosCurrentFrame = Input.mousePosition;

                mousePosLastFrame = mousePosCurrentFrame;
            }
        }
        */

        //如果点击鼠标左键并且没有点击在UI上
        if(Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit);
            ShowClickResult(currentClickType.Click(raycastHit));
        }

        if (Input.GetMouseButtonDown(2))
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

    public void ResetCamera()
    {
        transform.SetPositionAndRotation(originalPos, originalRotation);
        currentClickType.Reset();
    }

    public void SetCameraMode(CameraMode mode)
    {
        if (currentClickType != null) 
            currentClickType.Reset();
        currentCameraMode = mode;
        switch(mode)
        {
            case CameraMode.AimClick:currentClickType = aimClick;break;
            case CameraMode.DistanceMeasure:currentClickType = distanceMeasure;break;
            case CameraMode.AreaMeasure:currentClickType = areaMeasure;break;
        }
    }

    public void SetCameraMode(int index)
    {
        if (currentClickType != null)
            currentClickType.Reset();
        switch (index)
        {
            case 0:
                currentCameraMode = CameraMode.AimClick;
                currentClickType = aimClick;
                break;
            case 1:
                currentCameraMode = CameraMode.DistanceMeasure;
                currentClickType = distanceMeasure;
                break;
            case 2:
                currentCameraMode = CameraMode.AreaMeasure;
                currentClickType = areaMeasure;
                break;
        }
    }

    private void ShowClickResult(ClickResult clickResult)
    {
        if (clickResult == null) return;
        nameLabel.text = clickResult.ObjName;
        infoLabel.text = clickResult.ObjInfo;
    }
}
