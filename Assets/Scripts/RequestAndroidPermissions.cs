using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class RequestAndroidPermissions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RequestPermissions();
    }

    void RequestPermissions()
    {
        // Check if Camera permission has already been granted
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log("Requesting Camera permission..");
            Permission.RequestUserPermission(Permission.Camera);
        }
        else
        {
            Debug.Log("Camera permission already granted.");
        }

        // Check if GPD permission has already been granted
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("Requesting Location permission..");
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        else
        {
            Debug.Log("Location permission already granted.");
        }
    }
}
