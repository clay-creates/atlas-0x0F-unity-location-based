using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR;
using Niantic.Lightship.AR.WorldPositioning;
using TMPro;

public class GPSTracker : MonoBehaviour
{
    ARWorldPositioningCameraHelper cameraHelper;

    private double currentLongitude;
    private double currentLatitude;
    private double currentAltitude;

    public TMP_Text longitudeText;
    public TMP_Text latitudeText;
    public TMP_Text altitudeText;

    // Start is called before the first frame update
    void Start()
    {
        cameraHelper = Camera.main.GetComponent<ARWorldPositioningCameraHelper>();
    }

    // Update is called once per frame
    void Update()
    {
        currentLongitude = cameraHelper.Longitude;
        currentLatitude = cameraHelper.Latitude;
        currentAltitude = cameraHelper.Altitude;

        UpdateGPSText();
    }

    public void UpdateGPSText()
    {
        longitudeText.text = currentLongitude.ToString();
        latitudeText.text = currentLatitude.ToString();
        altitudeText.text = currentAltitude.ToString();
    }
}
