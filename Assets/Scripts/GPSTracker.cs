using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class GPSTracker : MonoBehaviour
{
    private double currentLongitude;
    private double currentLatitude;
    private double currentAltitude;

    public TMP_Text longitudeText;
    public TMP_Text latitudeText;
    public TMP_Text altitudeText;

    private float updateInterval = 1f;
    private float timer = 0f;

    public Button saveCoordButton;
    public Button requestCoordButton;
    public Button calculateDistanceButton;

    private string currentCoordinates = "";
    private string requestedCoordinates = "";
    private string distanceBetweenCoords = "";

    public TMP_Text requestedCoordinatesText;
    public TMP_Text savedCoordinatesText;
    public TMP_Text distanceBetweenCoordsText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeLocationService());

        saveCoordButton.onClick.AddListener(SaveCurrentCoords);
        requestCoordButton.onClick.AddListener(RequestNewCoords);
        calculateDistanceButton.onClick.AddListener(CalculateDistanceBetweenCoords);
    }

    private IEnumerator InitializeLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location Service is disabled by the user.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            Debug.LogError("Location service initialization timed out.");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location.");
            yield break;
        }

        Debug.Log("Location service initialized successfully.");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            timer += Time.deltaTime;

            if (timer >= updateInterval)
            {
                LocationInfo locationInfo = Input.location.lastData;
                currentLongitude = locationInfo.longitude;
                currentLatitude = locationInfo.latitude;
                currentAltitude = locationInfo.altitude;

                UpdateGPSText();
                timer = 0f;
            }
        }
        else
        {
            Debug.Log("Waiting for location service...");
        }
    }

    public void UpdateGPSText()
    {
        longitudeText.text = currentLongitude.ToString("F6");
        latitudeText.text = currentLatitude.ToString("F6");
        altitudeText.text = currentAltitude.ToString("F2");
    }

    public void SaveCurrentCoords()
    {

    }

    public void RequestNewCoords()
    {

    }

    public void CalculateDistanceBetweenCoords()
    {

    }
}
