using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SpatialCursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public DrivingSurfaceManager DrivingSurfaceManager;
    public ARPlane CurrentPlane;

    private Vector3 _hit_position;
    private bool has_hit = false;
    public Vector3 HitPosition
    {
        get => _hit_position;
    }

    public bool HasHit
    {
        get => has_hit;
    }

    void Update()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        var hits = new List<ARRaycastHit>();
        DrivingSurfaceManager.RaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinBounds);

        CurrentPlane = null;
        ARRaycastHit? hit = null;
        if (hits.Count > 0)
        {
            var lockedPlane = DrivingSurfaceManager.LockedPlane;
            hit = lockedPlane == null
                ? hits[0]
                : hits.SingleOrDefault(x => x.trackableId == lockedPlane.trackableId);
        }

        if (hit.HasValue)
        {
            CurrentPlane = DrivingSurfaceManager.PlaneManager.GetPlane(hit.Value.trackableId);
            _hit_position = hit.Value.pose.position;
        }
        has_hit = hit.HasValue;
    }
}
