using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraController : MonoBehaviour
{
    public RawImage rawImage; // 用于显示相机画面的 UI 组件
    public ARCameraManager arCameraManager; // ARCameraManager 用于获取摄像头数据

    public static CameraController instance;

    private Texture2D cameraTexture; // 用于存储当前相机帧数据
    private Texture2D photo; // 用于保存拍摄的照片

    private bool isProcessingPhoto = false; // 标志变量，用于控制是否处理帧

    void Start()
    {
        instance = this;

        if (arCameraManager == null)
        {
            Debug.LogError("ARCameraManager not set！");
            return;
        }

        // 订阅相机帧事件
        arCameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        // 如果未请求拍照，不处理帧
        if (!isProcessingPhoto) return;

        if (arCameraManager.TryAcquireLatestCpuImage(out var image))
        {
            using (image)
            {
                // 如果尚未初始化，创建适配分辨率的 Texture2D
                if (cameraTexture == null || cameraTexture.width != image.width || cameraTexture.height != image.height)
                {
                    cameraTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
                }

                // 转换 AR 相机帧数据为 Texture2D
                var conversionParams = new XRCpuImage.ConversionParams
                {
                    inputRect = new RectInt(0, 0, image.width, image.height),
                    outputDimensions = new Vector2Int(image.width, image.height),
                    outputFormat = TextureFormat.RGBA32,
                    transformation = XRCpuImage.Transformation.None
                };

                // 获取像素数据
                var rawTextureData = cameraTexture.GetRawTextureData<byte>();
                image.Convert(conversionParams, rawTextureData);
                cameraTexture.Apply();

                // 拍照完成，保存照片
                photo = new Texture2D(cameraTexture.width, cameraTexture.height, TextureFormat.RGBA32, false);
                photo.SetPixels(cameraTexture.GetPixels());
                photo.Apply();

                Debug.Log("finished");

                // 停止处理帧
                isProcessingPhoto = false;
            }
        }
    }

    public Texture2D TakePhoto()
    {
        if (isProcessingPhoto)
        {
            Debug.LogWarning("Processing photo, please wait ...");
            return null;
        }

        if (arCameraManager == null)
        {
            Debug.LogError("ARCameraManager Unassigned！");
            return null;
        }

        Debug.Log("requesting ...");
        isProcessingPhoto = true;

        // 等待一帧处理完成后返回照片
        StartCoroutine(WaitForPhoto());
        return photo;
    }

    private System.Collections.IEnumerator WaitForPhoto()
    {
        yield return new WaitUntil(() => !isProcessingPhoto);

        // 返回照片处理完成后的回调可以在这里触发
        Debug.Log("photo ready");
    }

    void OnDisable()
    {
        if (arCameraManager != null)
        {
            arCameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }
}
