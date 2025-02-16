using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using static ButtonHandler;

public class AnchorController : MonoBehaviour
{
    public List<string> ResolvingSet = new List<string>();
    public ARAnchorManager AnchorManager;
    public float Radius;
    public Camera MainCamera { get; set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateQualityState(int qualityState) {}
    public CloudAnchorHistoryCollection LoadCloudAnchorHistory() {
        return new CloudAnchorHistoryCollection();
    }

    [Serializable]
    public class CloudAnchorHistoryCollection
    {
        /// <summary>
        /// A list of Cloud Anchor History Data.
        /// </summary>
        public List<CloudAnchorHistory> Collection = new List<CloudAnchorHistory>();
    }
}
