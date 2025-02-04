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
    [SerializeField] private Button loadButton;
    
    public static event Action<string> saveEvent;

    private ARLocationManager _arLocationManager;
    private ARLocation _lastARLocation;

    private List<GameObject> spawnedObjects = new();
    [SerializeField] private ScriptableObjectID _scriptableObject;
    private Transform currentParentTransform;

    private string savePath;

    // Start is called before the first frame update
    void Start()
    {
        _arLocationManager = FindObjectOfType<ARLocationManager>();

        saveButton.onClick.AddListener(SaveCurrentMarkers);
        loadButton.onClick.AddListener(LoadSavedMarkers);

        PersistentObject.myPersistentData += SaveData;
        _arLocationManager.locationTrackingStateChanged += CheckIfShouldLoadData;

        savePath = Application.persistentDataPath;
    }

    private void OnDisable()
    {
        saveButton.onClick.RemoveAllListeners();
        loadButton.onClick.RemoveAllListeners();

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

    private void SaveCurrentMarkers()
    {
        if (spawnedObjects.Count == 0)
        {
            UnityEngine.Debug.LogWarning("No markers to save.");
            return;
        }

        string folderToSave = _arLocationManager.ARLocations[0].Payload.ToBase64();
        string combinedPath = Path.Combine(savePath, folderToSave);

        if (!Directory.Exists(combinedPath))
        {
            Directory.CreateDirectory(combinedPath);
        }

        foreach (GameObject obj in spawnedObjects)
        {
            PersistentObject persistentObject = obj.GetComponent<PersistentObject>();
            if (persistentObject != null)
            {
                PersistentObjectData objectData = new PersistentObjectData(
                        folderToSave,
                        persistentObject.PrefabID,
                        persistentObject.ObjectUUID,
                        obj.transform.position,
                        obj.transform.localScale,
                        obj.transform.rotation
                    );

                string filePath = Path.Combine(combinedPath, $"{persistentObject.ObjectUUID}.json");
                File.WriteAllText(filePath, JsonUtility.ToJson(objectData));

                UnityEngine.Debug.Log($"Saved marker at: {objectData._position}");
            }
        }
    }

    private void LoadSavedMarkers()
    {
        string folderToLoad = _arLocationManager.ARLocations[0].Payload.ToBase64();
        string combinedPath = Path.Combine(savePath, folderToLoad);

        if (!Directory.Exists(combinedPath))
        {
            UnityEngine.Debug.LogWarning("No saved markers found.");
            return;
        }

        UnityEngine.Debug.Log("Loading saved markers...");

        List<PersistentObjectData> objectsToSpawn = new();

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

        string folderToSave = objectData._locationID;
        string combinedPath = Path.Combine(savePath, folderToSave);

        if (!Directory.Exists(combinedPath))
        {
            Directory.CreateDirectory(combinedPath);
        }

        string filePath = Path.Combine(combinedPath, $"{objectData._uuid}.json");
        File.WriteAllText(filePath, JsonUtility.ToJson(objectData));

        UnityEngine.Debug.Log($"Saved data to: {filePath}");



#if UNITY_EDITOR
        OpenFolderInFinder(savePath);
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
