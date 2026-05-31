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
    public string Name;
    public string CustomerTypeName;
    public float WaitTime;
    public float MoveSpeed;
    public int RewardScore;

    public string NormalFrontSpriteId;
    public string NormalSideSpriteId;
    public string NormalBackSpriteId;

    public string BadFrontSpriteId;
    public string BadSideSpriteId;
    public string BadBackSpriteId;
    
    public string AngryFrontSpriteId;
    public string AngrySideSpriteId;
    public string AngryBackSpriteId;

    public CustomerType GetCustomerType()
    {
        if (Enum.TryParse(CustomerTypeName, out CustomerType customerType) == false)
        {
            return CustomerType.Normal;
        }

        return customerType;
    }
}

[Serializable]
public class CustomerDataList : GameDataBase
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
public class CustomerSpawnDataList : GameDataBase
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
public class RecipeDataList : GameDataBase
{
    public List<RecipeData> RecipeDatas;
}
