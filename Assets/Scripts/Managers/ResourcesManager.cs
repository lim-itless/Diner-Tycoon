using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Inst { get; private set; }

    private readonly Dictionary<string, GameObject> _prefabDictionary = new Dictionary<string, GameObject>();

    private readonly Dictionary<string, Sprite> _spriteDictionary = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
    }

    public GameObject LoadPrefab(string path)
    {
        if (_prefabDictionary.ContainsKey(path))
        {
            return _prefabDictionary[path];
        }

        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            return null;
        }

        _prefabDictionary.Add(path, prefab);

        return prefab;
    }

    public Sprite LoadSprite(string path)
    {
        if (_spriteDictionary.ContainsKey(path))
        {
            return _spriteDictionary[path];
        }

        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite == null)
        {
            return null;
        }

        _spriteDictionary.Add(path, sprite);

        return sprite;
    }
}