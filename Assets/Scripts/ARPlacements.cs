using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacements : MonoBehaviour
{
    private ARRaycastManager arRaycastManager;
    private ARAnchorManager arAnchorManager;
    [SerializeField] private GameObject markerPrefab;
    private List<GameObject> instantiatedObjects = new();
    private Camera mainCam;

    private static List<ARRaycastHit> hits = new();
    private const float prefabScale = 0.1f; // Adjust as needed

    void Start()
    {
        mainCam = Camera.main;
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arAnchorManager = FindObjectOfType<ARAnchorManager>();

        if (!arRaycastManager)
        {
            Debug.LogError("ARRaycastManager not found in scene.");
            enabled = false;
        }

        if (!arAnchorManager)
        {
            Debug.LogError("ARAnchorManager not found in scene.");
            enabled = false;
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI(Input.mousePosition))
        {
            TryPlaceObject(Input.mousePosition);
        }
#endif

#if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            if (!IsPointerOverUI(touch.position))
            {
                TryPlaceObject(touch.position);
            }
        }
#endif
    }

    private bool IsPointerOverUI(Vector2 position)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = position };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    void TryPlaceObject(Vector3 screenPosition)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPosition);

        if (arRaycastManager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose placementPose = hits[0].pose;

            // Create an empty GameObject at the placement pose
            GameObject anchorObject = new GameObject("AR Anchor");
            anchorObject.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);

            // Add an ARAnchor component to it
            ARAnchor anchor = anchorObject.AddComponent<ARAnchor>();

            if (anchor == null)
            {
                Debug.LogWarning("Failed to create an anchor.");
                return;
            }

            // Instantiate the markerPrefab as a child of the anchor
            GameObject newMarker = Instantiate(markerPrefab, anchor.transform);
            newMarker.transform.localScale = Vector3.one * prefabScale;

            instantiatedObjects.Add(newMarker);

            Debug.Log($"Marker placed at {placementPose.position} with ARAnchor.");
        }
        else
        {
            Debug.Log("No suitable plane detected for placement.");
        }
    }
}
