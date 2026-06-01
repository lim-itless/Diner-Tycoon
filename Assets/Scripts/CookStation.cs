using System.Collections.Generic;
using UnityEngine;

public class CookStation : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform FoodPoint;
    [SerializeField] private AudioClip _completeCookSFX;


    private readonly List<IngredientType> _ingredientList = new List<IngredientType>();
    private IngredientType _completedFoodType = IngredientType.None;
    private GameObject _currentFoodObject;

    private void RefreshFoodObject()
    {
        if (_currentFoodObject != null)
        {
            Destroy(_currentFoodObject);
        }

        if (_completedFoodType == IngredientType.None)
        {
            return;
        }

        GameObject foodPrefab = ResourceManager.Inst.LoadPrefab($"Prefabs/Food/Food_{_completedFoodType}");

        if (foodPrefab == null)
        {
            Debug.LogWarning($"{_completedFoodType} 프리팹 없음");
            return;
        }

        _currentFoodObject = Instantiate(foodPrefab, FoodPoint);
    }

    public void Interact(PlayerController playerController)
    {
        if (playerController.IsCarry == true)
        {
            AddIngredient(playerController.CurrentIngredientType);
            playerController.ClearIngredient();
            return;
        }

        TakeFood(playerController);
    }

    private void AddIngredient(IngredientType ingredientType)
    {
        if (ingredientType == IngredientType.None)
        {
            return;
        }

        if (_completedFoodType != IngredientType.None)
        {
            RuinFood();
        }

        _ingredientList.Add(ingredientType);

        Debug.Log($"조리대에 {ingredientType} 추가!");
        Debug.Log($"현재 개수 : {_ingredientList.Count}");

        CheckRecipe();
    }

    private void CheckRecipe()
    {
        RecipeData recipeData = GameDataManager.Inst.GetRecipeData(_ingredientList);

        if (recipeData == null)
        {
            return;
        }

        CompleteFood(recipeData.GetResultFoodType());
    }

    private void CompleteFood(IngredientType foodType)
    {
        _completedFoodType = foodType;
        SoundManager.Inst.PlaySFX(_completeCookSFX);
        RefreshFoodObject();
    }

    private void RuinFood()
    {
        _completedFoodType = IngredientType.MessFood;

        RefreshFoodObject();

        Debug.Log("괴상한 음식 완성!");
    }

    private void TakeFood(PlayerController playerController)
    {
        if (_completedFoodType == IngredientType.None)
        {
            Debug.Log("완성된 음식이 없습니다.");
            return;
        }

        playerController.CarryIngredient(_completedFoodType);

        Debug.Log($"{_completedFoodType} 들기!");

        _completedFoodType = IngredientType.None;
        _ingredientList.Clear();

        RefreshFoodObject();
    }

    public void ResetStation()
    {
        _ingredientList.Clear();
        _completedFoodType = IngredientType.None;

        if (_currentFoodObject == null)
        {
            return;
        }

        Destroy(_currentFoodObject);
        _currentFoodObject = null;
    }
}