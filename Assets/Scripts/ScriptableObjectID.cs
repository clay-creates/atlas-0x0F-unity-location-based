using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptablePrefabID", menuName = "ScriptableObjects/ScriptablePrefabID")]

public class ScriptableObjectID : ScriptableObject
{
    [SerializeField] private List<GameObject> persistentObjects;

    private Dictionary<string, GameObject> keyToObject;

    public void OnEnable()
    {
        keyToObject = new Dictionary<string, GameObject>();

        foreach (var objects in persistentObjects)
        {
            keyToObject.Add(objects.GetComponent<PersistentObject>().PrefabID, objects);
            Debug.Log($"Adding to Dictionary {objects.GetComponent<PersistentObject>().PrefabID} + {objects.name}");
        }
    }

    public GameObject ReturnObjectByID(string id)
    {
        return keyToObject[id];
    }
}
