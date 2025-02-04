using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.LocationAR;
using Niantic.Lightship.AR.PersistentAnchors;

public class VPSLocalization : MonoBehaviour
{
    private ARLocationManager _arLocationManager;
    public bool isVPSActive = false;

    // Start is called before the first frame update
    void Start()
    {
        _arLocationManager = FindObjectOfType<ARLocationManager>();
        _arLocationManager.locationTrackingStateChanged += OnVPSStateChanged;
    }

    void OnDestroy()
    {
        _arLocationManager.locationTrackingStateChanged -= OnVPSStateChanged;
    }

    void OnVPSStateChanged(ARLocationTrackedEventArgs eventArgs)
    {
        if (eventArgs.Tracking)
        {
            isVPSActive = true;
            Debug.Log("VPS Localization Successful!");
        }
        else
        {
            isVPSActive = false;
            Debug.Log("VPS tracking lost, switching to GPS.");
        }
    }
}
