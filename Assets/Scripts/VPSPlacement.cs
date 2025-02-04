using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Lightship.AR.PersistentAnchors;
using TMPro;

public class VPSPlacement : MonoBehaviour
{
    public GameObject markerPrefab;
    private ARPersistentAnchorManager _anchorManager;
    private VPSLocalization _vpsLocalization;

    // Start is called before the first frame update
    void Start()
    {
        _anchorManager = FindObjectOfType<ARPersistentAnchorManager>();
        _vpsLocalization = FindObjectOfType<VPSLocalization>();
    }

    public void PlaceObject(Vector3 position, string labelText)
    {
        GameObject newMarker = Instantiate(markerPrefab, position, Quaternion.identity);
        newMarker.transform.localScale = Vector3.one * 0.1f;

        // Attach text label to marker
        TextMeshPro textMesh = newMarker.GetComponentInChildren<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = labelText;
        }

        // VPS anchoring if available
        if (_vpsLocalization.isVPSActive)
        {
            Pose anchorPose = new Pose(position, Quaternion.identity);
            ARPersistentAnchor anchor;
            bool success = _anchorManager.TryCreateAnchor(anchorPose, out anchor);

            if (success && anchor != null)
            {
                Debug.Log("VPS anchor created successfully.");
            }
            else
            {
                Debug.LogWarning("Failed to create VPS anchor.");
            }
        }
        else
        {
            Debug.Log("VPS not available. Placing object without persistence.");
        }
    }
}
