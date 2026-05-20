using System.Collections.Generic;
using UnityEngine;

public class CookStation : MonoBehaviour, IInteractable
{
    private readonly List<IngredientType> _ingredientList = new List<IngredientType>();
    private IngredientType _completedFoodType = IngredientType.None;

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
        bool hasClam = _ingredientList.Contains(IngredientType.Clam);

        bool hasPotato = _ingredientList.Contains(IngredientType.Potato);

        bool hasMilk = _ingredientList.Contains(IngredientType.Milk);

        if (hasClam && hasPotato && hasMilk)
        {
            CompleteFood(IngredientType.ClamChowder);
            return;
        }

        if (hasPotato && hasMilk)
        {
            CompleteFood(IngredientType.PotatoSoup);
            return;
        }

        if (hasClam && hasPotato)
        {
            CompleteFood(IngredientType.PotatoMashInShell);
            return;
        }
    }

    private void CompleteFood(IngredientType foodType)
    {
        _completedFoodType = foodType;

        Debug.Log($"{foodType} 완성!");
    }

    private void RuinFood()
    {
        _completedFoodType = IngredientType.MessFood;

        Debug.Log($"괴상한 음식 완성!");
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
    }
}