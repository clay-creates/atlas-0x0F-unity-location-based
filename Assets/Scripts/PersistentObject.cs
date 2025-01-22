using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    [SerializeField] private string prefabID;
    public string PrefabID
    {
        get => prefabID;
        set => prefabID = value;
    }

    private string objectUUID;
    public string ObjectUUID
    {
        get => objectUUID;
        set => objectUUID = value;
    }

    public static event Action<PersistentObjectData> myPersistentData;

    // Start is called before the first frame update
    void Start()
    {
        if(string.IsNullOrEmpty(objectUUID))
            objectUUID = CreateUUID();

        SaveLoadAllObjects.saveEvent += SaveData;
    }

    private void OnDestroy()
    {
        SaveLoadAllObjects.saveEvent -= SaveData;
    }

    string CreateUUID()
    {
        return Guid.NewGuid().ToString();
    }

    void SaveData(string ARLocation)
    {
        PersistentObjectData objectData = new PersistentObjectData(
            ARLocation,
            prefabID,
            objectUUID,
            transform.position,
            transform.localScale,
            transform.rotation
            );

        myPersistentData?.Invoke( objectData );
    }
}

public struct PersistentObjectData
{
    public string _locationID;
    public string _prefabID;
    public string _uuid;

    public Vector3 _position;
    public Vector3 _localScale;
    public Quaternion _rotation;

    public PersistentObjectData(string locationID, string prefabID, string uuid, Vector3 position, Vector3 localScale, Quaternion rotation)
    {
        _locationID = locationID;
        _prefabID = prefabID;
        _uuid = uuid;
        _position = position;
        _localScale = localScale;
        _rotation = rotation;
    }
}