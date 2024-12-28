using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpatialPropertyManager : MonoBehaviour
{

    public Pose pose;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ApplyPose()
    {
        
    }

    // public void SetLocation(double latitude, double longitude, double lat_long_accuracy, double altitude, double altitude_accuracy)
    // {
    //     this.latitude = latitude;
    //     this.longitude = longitude;
    //     this.lat_long_accuracy = lat_long_accuracy;
    //     this.altitude = altitude;
    //     this.altitude_accuracy = altitude_accuracy;
    // }

    public void SetGeoPose(GeospatialPose geoPose)
    {
        throw new System.NotImplementedException();
    }

    public void SetPose(Pose pose)
    {
        this.pose = pose;
    }
}
