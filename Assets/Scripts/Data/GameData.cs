using JetBrains.Annotations;
using System;
using System.Collections.Generic;

[Serializable]
public class GameDataBase
{
    public string Id;
}

[Serializable]
public class CustomerData : GameDataBase
{
    public string CustomerId;
    public string CustomerName;
    public string CustomerTypeName;

    public string AnimatorControllerPath;

    public float WaitTime;
    public float MoveSpeed;
    public int RewardScore;
    
    public CustomerType GetCustomerType()
    {
        if (Enum.TryParse(CustomerName, out CustomerType customerType) == false)
        {
            return CustomerType.Normal;
        }

        return customerType;
    }
}

[Serializable]
public class CustomerDataList
{
    public List<CustomerData> CustomerDatas;
}

[Serializable]
public class CustomerSpawnData : GameDataBase
{
    public string CustomerId;
    public int SpawnWeight;
}

[Serializable]
public class CustomerSpawnDataList
{
    public List<CustomerSpawnData> CustomerSpawnDatas;
}

[Serializable]
public class RecipeData : GameDataBase
{
    public string RecipeId;
    public List<string> Ingredients;
    public string ResultFoodType;

    public IngredientType GetResultFoodType()
    {
        if (Enum.TryParse(ResultFoodType, out IngredientType foodType) == false)
        {
            return IngredientType.None;
        }

        return foodType;
    }
}

[Serializable]
public class RecipeDataList
{
    public List<RecipeData> RecipeDatas;
}
