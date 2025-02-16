using System;
using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class ButtonHandler : MonoBehaviour
{
    // public TMPro.TextMeshProUGUI DebugLogTextUI;
    // private SpatialCursor _cursor;
    public XROrigin xrOrigin;
    // private EarthDebugController _earthDebugController;
    // public UnityEngine.UI.RawImage rawImage;
    private ARAnchorManager ARAnchorManager;
    private AnchorController anchorController;
    public GameObject demoPrefab;
    public GameObject spatialImagePrefab;

    public List<ResolveCloudAnchorPromise> _resolvePromises = new();
    public List<ResolveCloudAnchorResult> _resolveResults = new();

    private HostCloudAnchorPromise _hostPromise;
    private HostCloudAnchorResult _hostResult;
    private CloudAnchorHistory _hostedCloudAnchor;
    private IEnumerator _hostCoroutine;
    private ARAnchor _anchor;
    private QualityIndicator _qualityIndicator;

    private void OnEnable()
    {
        ARAnchorManager = FindObjectOfType<ARAnchorManager>();

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

        Button takePhotoBtn2 = root.Q<Button>("btn2");
        if (takePhotoBtn2 != null)
        {
            // 绑定点击事件
            takePhotoBtn2.clicked += OnButton2Clicked;
        }
        else
        {
            Debug.LogError("Button not found!");
        }
    }

    void Update()
    {
        ResolvingCloudAnchors();
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

            CreatePlaneInView(photo, 0.7f, Camera.main);

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

    public List<Texture2D> textures = new List<Texture2D>();

    public void OnButton2Clicked()
    {
        var photo = textures.Count > 0 ? textures[0] : null;

        if (photo == null)
        {
            Debug.LogError("Photo is null, unable to apply texture.");
            return;
        }


        CreatePlaneInView(photo, 0.7f, Camera.main);

        // pop
        textures.RemoveAt(0);
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


    public void CreateCloudAnchor(Pose pose)
    {
        GameObject anchorObject = new GameObject("ARAnchor");
        anchorObject.transform.position = pose.position;
        anchorObject.transform.rotation = pose.rotation;

        // 直接给 GameObject 添加 ARAnchor 组件
        ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();

        if (anchor != null)
        {
            Debug.Log("Anchor successfully placed at: " + pose.position);
        }
        else
        {
            Debug.LogError("Failed to create anchor.");
        }
    }

    private void ResolvingCloudAnchors()
    {
        // No Cloud Anchor for resolving.
        if (anchorController.ResolvingSet.Count == 0)
        {
            return;
        }

        // There are pending or finished resolving tasks.
        if (_resolvePromises.Count > 0 || _resolveResults.Count > 0)
        {
            return;
        }

        // ARCore session is not ready for resolving.
        if (ARSession.state != ARSessionState.SessionTracking)
        {
            return;
        }

        Debug.LogFormat("Attempting to resolve {0} Cloud Anchor(s): {1}",
            anchorController.ResolvingSet.Count,
            string.Join(",", new List<string>(anchorController.ResolvingSet).ToArray()));
        foreach (string cloudId in anchorController.ResolvingSet)
        {
            var promise = anchorController.AnchorManager.ResolveCloudAnchorAsync(cloudId);
            if (promise.State == PromiseState.Done)
            {
                Debug.LogFormat("Faild to resolve Cloud Anchor " + cloudId);
                OnAnchorResolvedFinished(false, cloudId);
            }
            else
            {
                _resolvePromises.Add(promise);
                var coroutine = ResolveAnchor(cloudId, promise);
                StartCoroutine(coroutine);
            }
        }

        anchorController.ResolvingSet.Clear();
    }

    private void OnAnchorResolvedFinished(bool success, string cloudId, string response = null)
    {
        if (success)
        {
            // InstructionText.text = "Resolve success!";
            // DebugText.text =
            //     string.Format("Succeed to resolve the Cloud Anchor: {0}.", cloudId);
        }
        else
        {
            // InstructionText.text = "Resolve failed.";
            // DebugText.text = "Failed to resolve Cloud Anchor: " + cloudId +
            //     (response == null ? "." : "with error " + response + ".");
        }
    }

    private IEnumerator ResolveAnchor(string cloudId, ResolveCloudAnchorPromise promise)
    {
        yield return promise;
        var result = promise.Result;
        _resolvePromises.Remove(promise);
        _resolveResults.Add(result);

        if (result.CloudAnchorState == CloudAnchorState.Success)
        {
            OnAnchorResolvedFinished(true, cloudId);
            // Instantiate(CloudAnchorPrefab, result.Anchor.transform);
        }
        else
        {
            OnAnchorResolvedFinished(false, cloudId, result.CloudAnchorState.ToString());
        }
    }


    private void HostingCloudAnchor()
    {
        // There is no anchor for hosting.
        if (_anchor == null)
        {
            return;
        }

        // There is a pending or finished hosting task.
        if (_hostPromise != null || _hostResult != null)
        {
            return;
        }

        // Update map quality:
        int qualityState = 2;
        // Can pass in ANY valid camera pose to the mapping quality API.
        // Ideally, the pose should represent users’ expected perspectives.
        FeatureMapQuality quality =
            anchorController.AnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        // DebugText.text = "Current mapping quality: " + quality;
        qualityState = (int)quality;
        // _qualityIndicator.UpdateQualityState(qualityState);

        // // Hosting instructions:
        // var cameraDist = (_qualityIndicator.transform.position -
        //     anchorController.MainCamera.transform.position).magnitude;
        // if (cameraDist < _qualityIndicator.Radius * 1.5f)
        // {
        //     // InstructionText.text = "You are too close, move backward.";
        //     return;
        // }
        // else if (cameraDist > 10.0f)
        // {
        //     // InstructionText.text = "You are too far, come closer.";
        //     return;
        // }
        // else if (_qualityIndicator.ReachTopviewAngle)
        // {
        //     InstructionText.text =
        //         "You are looking from the top view, move around from all sides.";
        //     return;
        // }
        // else if (!_qualityIndicator.ReachQualityThreshold)
        // {
        //     InstructionText.text = "Save the object here by capturing it from all sides.";
        //     return;
        // }

        // Start hosting:
        // InstructionText.text = "Processing...";
        // DebugText.text = "Mapping quality has reached sufficient threshold, " +
        //     "creating Cloud Anchor.";
        // DebugText.text = string.Format(
        //     "FeatureMapQuality has reached {0}, triggering CreateCloudAnchor.",
        //     Controller.AnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose()));

        // Creating a Cloud Anchor with lifetime = 1 day.
        // This is configurable up to 365 days when keyless authentication is used.
        var promise = anchorController.AnchorManager.HostCloudAnchorAsync(_anchor, 1);
        if (promise.State == PromiseState.Done)
        {
            Debug.LogFormat("Failed to host a Cloud Anchor.");
            OnAnchorHostedFinished(false);
        }
        else
        {
            _hostPromise = promise;
            _hostCoroutine = HostAnchor();
            StartCoroutine(_hostCoroutine);
        }
    }

    private IEnumerator HostAnchor()
    {
        yield return _hostPromise;
        _hostResult = _hostPromise.Result;
        _hostPromise = null;

        if (_hostResult.CloudAnchorState == CloudAnchorState.Success)
        {
            int count = anchorController.LoadCloudAnchorHistory().Collection.Count;
            _hostedCloudAnchor =
                new CloudAnchorHistory("CloudAnchor" + count, _hostResult.CloudAnchorId);
            OnAnchorHostedFinished(true, _hostResult.CloudAnchorId);
        }
        else
        {
            OnAnchorHostedFinished(false, _hostResult.CloudAnchorState.ToString());
        }
    }


    private void OnAnchorHostedFinished(bool success, string response = null)
    {
        if (success)
        {
            // InstructionText.text = "Finish!";
            Invoke("DoHideInstructionBar", 1.5f);
            // DebugText.text =
            //     string.Format("Succeed to host the Cloud Anchor: {0}.", response);

            // Display name panel and hide instruction bar.
            // NameField.text = _hostedCloudAnchor.Name;
            // NamePanel.SetActive(true);
            // SetSaveButtonActive(true);
        }
        else
        {
            // InstructionText.text = "Host failed.";
            // DebugText.text = "Failed to host a Cloud Anchor" + (response == null ? "." :
            //     "with error " + response + ".");
        }
    }

    public Pose GetCameraPose()
    {
        return new Pose(anchorController.MainCamera.transform.position,
            anchorController.MainCamera.transform.rotation);
    }

    [Serializable]
    public struct CloudAnchorHistory
    {

        public string Name;

        public string Id;

        public string SerializedTime;

        public CloudAnchorHistory(string name, string id, DateTime time)
        {
            Name = name;
            Id = id;
            SerializedTime = time.ToString();
        }

        public CloudAnchorHistory(string name, string id) : this(name, id, DateTime.Now)
        {
        }

        public DateTime CreatedTime
        {
            get
            {
                return Convert.ToDateTime(SerializedTime);
            }
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }


}
