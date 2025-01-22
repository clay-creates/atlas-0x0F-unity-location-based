using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.AR.LocationAR;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using Niantic.Lightship.AR.PersistentAnchors;

public class SaveLoadAllObjects : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    
    public static event Action<string> saveEvent;

    private ARLocationManager _arLocationManager;
    private ARLocation _lastARLocation;

    List<GameObject> spawnedObjects;
    [SerializeField] ScriptableObjectID _scriptableObject;
    private Transform currentParentTransform;

    // Start is called before the first frame update
    void Start()
    {
        _arLocationManager = FindObjectOfType<ARLocationManager>();
        saveButton.onClick.AddListener(call: () => saveEvent?.Invoke(_arLocationManager.ARLocations[0].Payload.ToBase64()));
        PersistentObject.myPersistentData += SaveData;
        _arLocationManager.locationTrackingStateChanged += CheckIfShouldLoadData;
    }

    private void OnDisable()
    {
        saveButton.onClick.RemoveAllListeners();
        PersistentObject.myPersistentData -= SaveData;
        _arLocationManager.locationTrackingStateChanged -= CheckIfShouldLoadData;
    }

    void CheckIfShouldLoadData(ARLocationTrackedEventArgs eventArgs)
    {
        if (!eventArgs.Tracking)
        {
            return;
        }

        if (_lastARLocation == null || eventArgs.ARLocation != _lastARLocation)
        {
            if (eventArgs.ARLocation != null)
                LoadData(eventArgs.ARLocation);
        }
        else
        {
            UnityEngine.Debug.Log("Location already loaded");
        }

    }

    void LoadData(ARLocation location)
    {
        UnityEngine.Debug.Log("Loading data...");

        string pathToLoad = Application.persistentDataPath;
        string folderToLoad = location.Payload.ToBase64();
        string combinedPath = Path.Combine(pathToLoad + "/" + folderToLoad);

        currentParentTransform = FindObjectOfType<ARLocation>().transform;
        if (currentParentTransform == null)
        {
            return;
        }

        List<PersistentObjectData> objectsToSpawn = new();

        if (Directory.Exists(combinedPath))
        {
            foreach (var file in Directory.GetFiles(combinedPath))
            {
                string readFile = File.ReadAllText(file);
                objectsToSpawn.Add(JsonUtility.FromJson<PersistentObjectData>(readFile));
            }

            foreach (var spawnObject in objectsToSpawn)
            {
                SpawnObjectsFromPersistentObjectData(spawnObject);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Nothing has been placed here yet");
        }
    }

    // After Scriptable Object, spawn object
    void SpawnObjectsFromPersistentObjectData(PersistentObjectData data)
    {
        spawnedObjects = new List<GameObject>();
        GameObject toSpawn = Instantiate(_scriptableObject.ReturnObjectByID(data._prefabID));
        toSpawn.GetComponent<PersistentObject>().ObjectUUID = data._uuid;

        if (currentParentTransform != null)
        {
            toSpawn.transform.parent = currentParentTransform;
        }

        toSpawn.transform.position = data._position;
        toSpawn.transform.rotation = data._rotation;
        toSpawn.transform.localScale = data._localScale;
   
    }

    void SaveData(PersistentObjectData objectData)
    {

        string pathToSave = Application.persistentDataPath;
        string folderToSave = objectData._locationID;
        string combinedPath = Path.Combine(pathToSave + "/" + folderToSave);

        if(Directory.Exists(combinedPath))
        {
            File.WriteAllText(combinedPath + "/" + $"{objectData._uuid}json", JsonUtility.ToJson(objectData));
        }
        else
        {
            Directory.CreateDirectory(combinedPath);
            File.WriteAllText(combinedPath + "/" + $"{objectData._uuid}json", JsonUtility.ToJson(objectData));
        }

        

#if UNITY_EDITOR
        OpenFolderInFinder(pathToSave);
#endif

    }

    public void OpenFolderInFinder(string folderPath)
    {
        // Ensure the folder path is correct and exists
        if (System.IO.Directory.Exists(folderPath))
        {
            // Open the folder
            Process.Start(new ProcessStartInfo()
            {
                FileName = folderPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        else
        {
            UnityEngine.Debug.LogError("Folder not found: " + folderPath);
        }
    }
}
