using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Inst { get; private set; }

    private readonly Dictionary<string, CustomerData> _customerDataDictionary = new Dictionary<string, CustomerData>();

    private readonly List<CustomerSpawnData> _customerSpawnDatas = new List<CustomerSpawnData>();

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;

        LoadCustomerData();
        LoadCustomerSpawnData();
    }

    private void LoadCustomerData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("JsonOutput/Customer");

        if (textAsset == null)
        {
            Debug.LogError("Customer Json을 찾을 수 없습니다.");
            return;
        }

        string jsonText = "{\"CustomerDatas\":" + textAsset.text + "}";

        CustomerDataList dataList = JsonUtility.FromJson<CustomerDataList>(jsonText);

        if (dataList == null || dataList.CustomerDatas == null)
        {
            Debug.LogError("Customer Json 파싱 실패");
            return;
        }

        for (int i = 0; i < dataList.CustomerDatas.Count; i++)
        {
            CustomerData data = dataList.CustomerDatas[i];

            if (data == null || string.IsNullOrEmpty(data.Id) == true)
            {
                continue;
            }

            if (_customerDataDictionary.ContainsKey(data.Id) == true)
            {
                Debug.LogWarning($"중복 Customer Id : {data.Id}");
                continue;
            }

            _customerDataDictionary.Add(data.Id, data);
        }
    }

    private void LoadCustomerSpawnData()
    {
        TextAsset textAsset =
            Resources.Load<TextAsset>("JsonOutput/CustomerSpawnData");

        if (textAsset == null)
        {
            Debug.LogError("CustomerSpawnData Json을 찾을 수 없습니다.");
            return;
        }

        string jsonText = "{\"CustomerSpawnDatas\":" + textAsset.text + "}";

        CustomerSpawnDataList dataList = JsonUtility.FromJson<CustomerSpawnDataList>(jsonText);

        if (dataList == null || dataList.CustomerSpawnDatas == null)
        {
            Debug.LogError("CustomerSpawnTable Json 파싱 실패");
            return;
        }

        _customerSpawnDatas.Clear();
        _customerSpawnDatas.AddRange(dataList.CustomerSpawnDatas);
    }

    public CustomerData GetCustomerData(string id)
    {
        if (string.IsNullOrEmpty(id) == true)
        {
            Debug.LogWarning("Customer Id가 비어있습니다.");
            return null;
        }

        if (_customerDataDictionary.ContainsKey(id) == false)
        {
            Debug.LogWarning($"{id} CustomerData가 없습니다.");
            return null;
        }

        return _customerDataDictionary[id];
    }

    public CustomerData GetRandomCustomerData()
    {
        if (_customerSpawnDatas.Count == 0)
        {
            Debug.LogWarning("CustomerSpawnData가 비어있습니다.");
            return null;
        }

        int totalWeight = 0;

        for (int i = 0; i < _customerSpawnDatas.Count; i++)
        {
            if (_customerSpawnDatas[i] == null)
            {
                continue;
            }

            totalWeight += _customerSpawnDatas[i].Weight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning("CustomerSpawnData Weight 합계가 0 이하입니다.");
            return null;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        for (int i = 0; i < _customerSpawnDatas.Count; i++)
        {
            CustomerSpawnData spawnData = _customerSpawnDatas[i];

            if (spawnData == null)
            {
                continue;
            }

            currentWeight += spawnData.Weight;

            if (randomValue < currentWeight)
            {
                return GetCustomerData(spawnData.CustomerId);
            }
        }

        return null;
    }
}