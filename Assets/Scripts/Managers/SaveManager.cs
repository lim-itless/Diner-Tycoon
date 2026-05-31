using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Inst { get; private set; }

    private const string SAVE_FILE_NAME = "SaveData.json";

    public SaveData SaveData { get; private set; }
    public GameResultModel GameResultModel { get; private set; }

    private int _score;
    private bool _isGameOver;
    private float _remainingTime;

    private string SavePath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        }
    }

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;

        LoadGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(SaveData, true);

        File.WriteAllText(SavePath, json);
    }

    public void LoadGame()
    {
        if (File.Exists(SavePath) == false)
        {
            SaveData = new SaveData();
            SaveGame();
            return;
        }

        string json = File.ReadAllText(SavePath);

        SaveData = JsonUtility.FromJson<SaveData>(json);

        if (SaveData != null)
        {
            return;
        }

        SaveData = new SaveData();
    }
}