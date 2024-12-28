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

    private void OnButtonClicked()
    {
        var texture = CameraController.instance.TakePhoto();

        // rawImage.texture = texture;
        // create a spatial image use this texture and current geo pose

        // var obj = new GameObject("Package Prefab");
        var obj = Instantiate(demoPrefab);
        obj.transform.position = Camera.main.transform.position;
        obj.transform.rotation = Camera.main.transform.rotation;

        obj.SetActive(true);
        // var spatialImage = obj.AddComponent<SpatialImage>();
        // spatialImage.ApplyImage(texture);
        // var spatialPropertyManager = obj.AddComponent<SpatialPropertyManager>();
        // spatialPropertyManager.SetPose(new Pose(Camera.main.transform.position, Camera.main.transform.rotation));
        // spatialPropertyManager.SetGeoPose(_earthDebugController.GeoPose);

        Debug.Log($"Placing image at {Camera.main.transform.position}");
        Debug.Log($"OriginInCameraSpacePos={xrOrigin.OriginInCameraSpacePos}");
        Debug.Log($"CameraInOriginSpacePos={xrOrigin.CameraInOriginSpacePos}");
        Debug.Log($"Origin.transform.position={xrOrigin.Origin.transform.position}");

    }
}
