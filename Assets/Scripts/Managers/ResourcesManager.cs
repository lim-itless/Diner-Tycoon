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

    public Sprite LoadSprite(string spriteId)
    {
        if (string.IsNullOrEmpty(spriteId) == true)
        {
            return null;
        }

        int lastSlashIndex = spriteId.LastIndexOf('/');

        if (lastSlashIndex < 0)
        {
            return Resources.Load<Sprite>(spriteId);
        }

        string sheetPath = spriteId.Substring(0, lastSlashIndex);
        string spriteName = spriteId.Substring(lastSlashIndex + 1);

        Sprite[] sprites = Resources.LoadAll<Sprite>(sheetPath);

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name == spriteName)
            {
                return sprites[i];
            }
        }

        Debug.LogWarning($"Sprite 로드 실패 : {spriteId}");
        return null;
    }
}