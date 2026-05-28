using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager Inst { get; private set; }

    private int _nextInstanceId = 1;
    private readonly Dictionary<int, GameObject> _objectDictionary = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
    }

    public int CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            return -1;
        }

        GameObject createdObject = Instantiate(prefab, position, rotation);

        int instanceId = CreateInstanceId();
        _objectDictionary.Add(instanceId, createdObject);

        return instanceId;
    }

    public GameObject GetObject(int instanceId)
    {
        if (_objectDictionary.ContainsKey(instanceId) == false)
        {
            return null;
        }

        return _objectDictionary[instanceId];
    }

    public void RemoveObject(int instanceId)
    {
        if (_objectDictionary.ContainsKey(instanceId) == false)
        {
            return;
        }

        GameObject targetObject = _objectDictionary[instanceId];

        _objectDictionary.Remove(instanceId);
        Destroy(targetObject);
    }

    public void RemoveObject(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return;
        }

        int removeInstanceId = -1;

        foreach (KeyValuePair<int, GameObject> pair in _objectDictionary)
        {
            if (pair.Value == targetObject)
            {
                removeInstanceId = pair.Key;
                break;
            }
        }

        if (removeInstanceId < 0)
        {
            Destroy(targetObject);
            return;
        }

        _objectDictionary.Remove(removeInstanceId);
        Destroy(targetObject);
    }

    private int CreateInstanceId()
    {
        int instanceId = _nextInstanceId;
        _nextInstanceId++;

        return instanceId;
    }
}