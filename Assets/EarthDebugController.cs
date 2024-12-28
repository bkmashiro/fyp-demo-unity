using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class EarthDebugController : MonoBehaviour
{
    private AREarthManager _earthManager;
    // public Text EarthStatusTextUI;
    public TMPro.TextMeshProUGUI EarthStatusTextUI;
    public ARCoreExtensions ARCoreExtensions;

    private GeospatialPose _geoPose;

    public GeospatialPose GeoPose
    {
        get => _geoPose;
    }

    void Awake()
    {
        _earthManager = GetComponent<AREarthManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        string txt = "";
        if (ARSession.state != ARSessionState.SessionTracking)
        {
            txt += "[ERROR] ARSession.state == " + ARSession.state;
        }

        if (EarthStatusTextUI == null || _earthManager == null)
        {
            txt += "[ERROR] EarthStatusTextUI == null || _earthManager == null";
        }

        if (txt != "")
        {
            EarthStatusTextUI.text = txt;
            return;
        }

        FeatureSupported geospatialIsSupported = _earthManager.IsGeospatialModeSupported(
            ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode);
        if (geospatialIsSupported == FeatureSupported.Unknown)
        {
            EarthStatusTextUI.text = "[ERROR] Geospatial supported is unknown";
            return;
        }
        else if (geospatialIsSupported == FeatureSupported.Unsupported)
        {
            EarthStatusTextUI.text = string.Format(
                "[ERROR] GeospatialMode {0} is unsupported on this device.",
                ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode);
            return;
        }

        EarthState earthState = _earthManager.EarthState;
        if (earthState != EarthState.Enabled)
        {
            EarthStatusTextUI.text = "[ERROR] EarthState: " + earthState;
            return;
        }

        TrackingState trackingState = _earthManager.EarthTrackingState;
        if (trackingState != TrackingState.Tracking)
        {
            EarthStatusTextUI.text = "[ERROR] EarthTrackingState: " + trackingState;
            return;
        }

        GeospatialPose geoPose = _earthManager.CameraGeospatialPose;
        EarthStatusTextUI.text = string.Format(
            "Earth Tracking State:   - TRACKING -\n" +
            "LAT/LNG: {0:0.00000}, {1:0.00000} (acc: {2:0.000})\n" +
            "ALTITUDE: {3:0.0}m (acc: {4:0.0}m)\n" +
            "HEADING:{5:0.0}º (acc: {6:0.0}º)",
            geoPose.Latitude, geoPose.Longitude,
            geoPose.HorizontalAccuracy,
            geoPose.Altitude, geoPose.VerticalAccuracy,
            geoPose.EunRotation, geoPose.OrientationYawAccuracy);

        _geoPose = geoPose;
    }
}
