using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    Move,
    MeasureDis,
    MeasureArea
}

public class CameraController : MonoBehaviour
{
    public CameraMode cameraMode = CameraMode.Move;

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
    private GameObject target;
    private int originalLayer;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        originalRotation = transform.rotation;
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

        if(Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit);
            SetOutLine(raycastHit);
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

    private void FixedUpdate()
    {
        
    }

    public void ResetCamera()
    {
        transform.SetPositionAndRotation(originalPos, originalRotation);
        //记得重置target
    }

    public void SetOutLine(RaycastHit raycastHit)
    {
        /*
        if (raycastHit.collider != null)
        {
            GameObject obj = raycastHit.collider.gameObject;
            if (obj == target) return;
            //还原之前的materials
            if (target != null)
                target.GetComponent<MeshRenderer>().materials = originalMaterials;
            //存储当前的materials
            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            originalMaterials = mr.materials;
            //开始新建新的materials
            Material[] newMaterials = new Material[mr.materials.Length];
            for (int i = 0; i < mr.materials.Length; i++)
            {
                newMaterials[i] = new Material(Shader.Find("Custom/AlphaOutLine"));
                newMaterials[i].color = mr.materials[i].color;
            }
            mr.materials = newMaterials;
            target = obj;
        }
        */
        if (raycastHit.collider != null)
        {
            GameObject obj = raycastHit.collider.gameObject;
            if (obj == target) return;
            if (target != null)
                target.layer = originalLayer;
            originalLayer = obj.layer;
            obj.layer = LayerMask.NameToLayer("OutLine");
            target = obj;
        }
        
    }

    public void SetCameraMode(CameraMode mode)
    {
        cameraMode = mode;
    }

}
