using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Inst { get; private set; }

    private void Awake()
    {
        Inst = this;

        _customerDataDictionary = LoadData<CustomerData>("Customer");
        //_customerSpawnDataDictionary = LoadData<CustomerSpawnData>("CustomerSpawnData");
        //_recipeDataDictionary = LoadData<RecipeData>("RecipeData");

        LoadCustomerSpawnData();
        LoadRecipeData();
    }

    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> items;
    }

    private Dictionary<string, CustomerData> _customerDataDictionary = new Dictionary<string, CustomerData>();
    //private Dictionary<string, RecipeData> _recipeDataDictionary = new Dictionary<string, RecipeData>();
    //private Dictionary<string, CustomerSpawnData> _customerSpawnDataDictionary = new Dictionary<string, CustomerSpawnData>();
    
    private readonly List<CustomerSpawnData> _customerSpawnDatas = new List<CustomerSpawnData>();
    private readonly List<RecipeData> _recipeDatas = new List<RecipeData>();

    private Dictionary<string, T> LoadData<T>(string tableName) where T : GameDataBase
    {
        string resourcePath = $"JsonOutput/{tableName}";

        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

        if (textAsset == null)
        {
            Debug.LogError($"[Error] 리소스를 찾을 수 없습니다: Resources/{resourcePath}");
            return new Dictionary<string, T>();
        }

        try
        {
            string jsonString = textAsset.text;

            string wrappedJson = "{\"items\":" + jsonString + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if (wrapper != null && wrapper.items != null)
            {
                Debug.Log($"{typeof(T).Name} 데이터를 {wrapper.items.Count}개 로드했습니다.");
                return wrapper.items.ToDictionary(item => item.Id.ToString());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{typeof(T).Name} JSON 로드 오류] {ex.Message}");
        }

        return new Dictionary<string, T>();
    }

    private void LoadCustomerSpawnData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("JsonOutput/CustomerSpawnData");

        if (textAsset == null)
        {
            Debug.LogError("CustomerSpawnData 못 찾음");
            return;
        }
        
        string jsonText = "{\"CustomerSpawnDatas\":" + textAsset.text + "}";

        CustomerSpawnDataList dataList = JsonUtility.FromJson<CustomerSpawnDataList>(jsonText);

        if (dataList == null || dataList.CustomerSpawnDatas == null)
        {
            Debug.LogError("CustomerSpawnData Json 파싱 실패");
            return;
        }

        _customerSpawnDatas.Clear();
        _customerSpawnDatas.AddRange(dataList.CustomerSpawnDatas);

        Debug.Log($"CustomerSpawnData 로드 개수 : {_customerSpawnDatas.Count}");
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

    private void LoadRecipeData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("JsonOutput/RecipeData");

        if (textAsset == null)
        {
            Debug.LogError("RecipeData Json 못 찾음!");
            return;
        }

        string jsonText = "{\"RecipeDatas\":" + textAsset.text + "}";

        RecipeDataList dataList = JsonUtility.FromJson<RecipeDataList>(jsonText);

        if (dataList == null || dataList.RecipeDatas == null)
        {
            Debug.LogError("RecipeData Json 파싱 실패");
            return;
        }

        _recipeDatas.Clear();
        _recipeDatas.AddRange(dataList.RecipeDatas);
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
            CustomerSpawnData spawnData = _customerSpawnDatas[i];

            if (spawnData == null)
            {
                continue;
            }

            totalWeight += spawnData.SpawnWeight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning("CustomerSpawnData Weight 합계가 0 이하입니다.");
            return null;
        }

        int randomValue =
            UnityEngine.Random.Range(0, totalWeight);

        int currentWeight = 0;

        for (int i = 0; i < _customerSpawnDatas.Count; i++)
        {
            CustomerSpawnData spawnData =
                _customerSpawnDatas[i];

            if (spawnData == null)
            {
                continue;
            }

            currentWeight += spawnData.SpawnWeight;

            if (randomValue < currentWeight)
            {
                return GetCustomerData(
                    spawnData.CustomerId
                );
            }
        }

        return null;
    }

    public RecipeData GetRecipeData(List<IngredientType> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < _recipeDatas.Count; i++)
        {
            RecipeData recipeData = _recipeDatas[i];

            if (IsSameRecipe(recipeData, ingredients) == true)
            {
                return recipeData;
            }
        }

        return null;
    }

        private bool IsSameRecipe(RecipeData recipeData, List<IngredientType> ingredients)
        {
            if (recipeData == null || recipeData.Ingredients == null)
            {
                return false;
            }

            if (recipeData.Ingredients.Count != ingredients.Count)
            {
                return false;
            }

            for (int i = 0; i < ingredients.Count; i++)
            {
                string ingredientName = ingredients[i].ToString();

                if (recipeData.Ingredients.Contains(ingredientName) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }