using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.LocationAR;
using UnityEngine.XR.ARFoundation;
using Niantic.Lightship.AR.PersistentAnchors;

public class EnablePlacementOnLocalized : MonoBehaviour
{
    private ARLocationManager _arLocationManager;
    private ARPlaneManager _planeManager;
    private ARPlacements _arPlacements;

    // Start is called before the first frame update
    void Start()
    {
        _arLocationManager = GetComponent<ARLocationManager>();
        _planeManager = GetComponent<ARPlaneManager>();
        _arPlacements = GetComponent<ARPlacements>();

        _planeManager.enabled = false;
        _arPlacements.enabled = false;

        _arLocationManager.locationTrackingStateChanged += OnLocalized;
    }

    private void OnDestroy()
    {
        _arLocationManager.locationTrackingStateChanged -= OnLocalized;
    }

    void OnLocalized(ARLocationTrackedEventArgs eventArgs)
    {
        if (eventArgs.Tracking)
        {
            _planeManager.enabled = true;
            _arPlacements.enabled = true;
        }
        else
        {
            _planeManager.enabled = false;
            _arPlacements.enabled = false;
        }
    }
}
