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

    public Button saveCoordButton;
    public Button requestCoordButton;
    public Button calculateDistanceButton;

    private Vector2 currentCoordinates = Vector2.zero;
    private Vector2 savedCoordinates = Vector2.zero;
    private string distanceBetweenCoords = "";
    private string unityLocalPosition = "";

    public TMP_Text currentCoordinatesText;
    public TMP_Text savedCoordinatesText;
    public TMP_Text distanceBetweenCoordsText;
    public TMP_Text unityLocalPositionText;

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
            LocationInfo locationInfo = Input.location.lastData;
            currentLongitude = locationInfo.longitude;
            currentLatitude = locationInfo.latitude;
            currentAltitude = locationInfo.altitude;

            UpdateGPSText();
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
        savedCoordinates = new Vector2((float)currentLatitude, (float)currentLongitude);
        string formattedSavedCoords = FormatCoordinates(savedCoordinates.x, savedCoordinates.y);
        savedCoordinatesText.text = $"{formattedSavedCoords}";
        Debug.Log($"Coordinates saved: {formattedSavedCoords}");

        GPSEncoder.SetLocalOrigin(savedCoordinates);
    }

    public void RequestNewCoords()
    {
        string formattedCurrentCoords = FormatCoordinates(currentLatitude, currentLongitude);
        currentCoordinatesText.text = $"{formattedCurrentCoords}";
        Debug.Log($"Requested Coordinates: {formattedCurrentCoords}");

        TransformGPSToULP();
    }

    public void CalculateDistanceBetweenCoords()
    {
        if (savedCoordinates == Vector2.zero)
        {
            Debug.Log("No saved coordinates to calculate distance");
            distanceBetweenCoordsText.text = "No saved location.";
            return;
        }

        Vector2 currentCoords = new Vector2((float)currentLatitude, (float)currentLongitude);
        float distance = CalculateHaversineDistance(savedCoordinates, currentCoords);

        distanceBetweenCoords = $"{distance:F2} meters";
        distanceBetweenCoordsText.text = $"{distanceBetweenCoords}";
        Debug.Log($"Distance between coordinates: {distanceBetweenCoords}");
    }

    private string FormatCoordinates(double latitude, double longitude)
    {
        string latDirection = latitude >= 0 ? "N" : "S";
        string lonDirection = longitude >= 0 ? "E" : "W";

        return $"{Mathf.Abs((float)latitude):F4}° {latDirection}, {Mathf.Abs((float)longitude):F4}° {lonDirection}";
    }

    private float CalculateHaversineDistance(Vector2 coord1, Vector2 coord2)
    {
        float R = 6371000;
        float lat1Rad = Mathf.Deg2Rad * coord1.x;
        float lat2Rad = Mathf.Deg2Rad * coord2.x;
        float deltaLat = Mathf.Deg2Rad * (coord2.x - coord1.x);
        float deltaLon = Mathf.Deg2Rad * (coord2.y - coord1.y);

        float a = Mathf.Sin(deltaLat / 2) * Mathf.Sin(deltaLat / 2) +
            Mathf.Cos(lat1Rad) * Mathf.Cos(lat2Rad) *
            Mathf.Sin(deltaLon / 2) * Mathf.Sin(deltaLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return R * c;
    }

    public void TransformGPSToULP()
    {
        if (savedCoordinates == Vector2.zero)
        {
            Debug.Log("Local origin not set. Please save coordinates first.");
            unityLocalPositionText.text = "No saved location.";
            return;
        }

        Vector2 gps = new Vector2((float)currentLongitude, (float)currentLatitude);
        Vector3 ucsPosition = GPSEncoder.GPSToUCS(gps);
        unityLocalPositionText.text = $"X: {ucsPosition.x:F2}, Y: {ucsPosition.y:F2}, Z: {ucsPosition.z:F2}";

        Debug.Log($"Unity Local Position: {ucsPosition}");
    }
}
