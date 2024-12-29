using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonHandler : MonoBehaviour
{
    public TMPro.TextMeshProUGUI DebugLogTextUI;
    // private SpatialCursor _cursor;
    public XROrigin xrOrigin;
    // private EarthDebugController _earthDebugController;
    // public UnityEngine.UI.RawImage rawImage;

    public GameObject demoPrefab;
    public GameObject spatialImagePrefab;
    private void OnEnable()
    {
        if (xrOrigin == null)
        {
            xrOrigin = FindObjectOfType<XROrigin>();
        }
        // _earthDebugController = FindObjectOfType<EarthDebugController>();
        // _cursor = GetComponent<SpatialCursor>();
        // 获取UI Document
        var uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument not found!");
            return;
        }

        // 获取根VisualElement
        VisualElement root = uiDocument.rootVisualElement;

        // 获取按钮
        Button takePhotoBtn = root.Q<Button>("takePhoto");
        if (takePhotoBtn != null)
        {
            // 绑定点击事件
            takePhotoBtn.clicked += OnButtonClicked;
        }
        else
        {
            Debug.LogError("Button not found!");
        }
    }


    public async void OnButtonClicked()
    {
        try
        {
            // 请求拍照并等待结果
            var photo = await FindObjectOfType<CameraSnapshotManager>().TakePhotoAsync();

            if (photo == null)
            {
                Debug.LogError("Photo is null, unable to apply texture.");
                return;
            }

            // 创建物体并应用材质
            // var img = Instantiate(spatialImagePrefab);
            // img.transform.position = Camera.main.transform.position;
            // img.transform.rotation = Camera.main.transform.rotation;
            // img.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = photo;

            // img.SetActive(true);

            CreatePlaneInView(photo, 2f, Camera.main);

            Debug.Log($"Placing image at {Camera.main.transform.position}");
            Debug.Log($"OriginInCameraSpacePos={xrOrigin.OriginInCameraSpacePos}");
            Debug.Log($"CameraInOriginSpacePos={xrOrigin.CameraInOriginSpacePos}");
            Debug.Log($"Origin.transform.position={xrOrigin.Origin.transform.position}");

        }
        catch (Exception ex)
        {
            Debug.LogError($"Error capturing photo: {ex}");
        }
    }



    public void CreatePlaneInView(Texture2D texture, float distance, Camera mainCamera)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not assigned!");
            return;
        }

        if (texture == null)
        {
            Debug.LogError("Texture is null!");
            return;
        }

        // 计算平面位置：距离摄像机 N 米
        Vector3 planePosition = mainCamera.transform.position + mainCamera.transform.forward * distance;

        // 创建平面实例
        GameObject plane = Instantiate(spatialImagePrefab);
        plane.transform.position = planePosition;

        // 设置平面朝向：正对摄像机
        plane.transform.rotation = Quaternion.LookRotation(plane.transform.position - mainCamera.transform.position);

        // 计算平面大小以匹配摄像机视野
        float height = 2f * distance * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float width = height * mainCamera.aspect;

        // 设置平面大小
        plane.transform.localScale = new Vector3(width, height, 1f);

        // 应用材质
        Renderer renderer = plane.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = texture;
        }
        else
        {
            Debug.LogError("Plane prefab does not have a Renderer component!");
        }
        plane.SetActive(true);
        Debug.Log($"Created plane at distance {distance} meters with size {width}x{height}");
    }
}
