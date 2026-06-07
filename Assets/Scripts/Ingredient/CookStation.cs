using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookStation : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform FoodPoint;
    [SerializeField] private SpriteRenderer[] SpriteRenderer_IngredientIcons;
    [SerializeField] private Sprite EmptyIcon;
    [SerializeField] private AudioClip _completeCookSFX;
    [SerializeField] private GameObject CompleteEffectPrefab;
    [SerializeField] private float _foodPopDuration = 0.15f;
    [SerializeField] private float _foodPopStartScale = 0.6f;
    [SerializeField] private float _foodPopEndScale = 1.15f;

    private Coroutine _foodPopCoroutine;
    private readonly List<IngredientType> _ingredientList = new List<IngredientType>();
    private IngredientType _completedFoodType = IngredientType.None;
    private GameObject _currentFoodObject;
    private const int MAX_INGREDIENT_COUNT = 4;

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

        PlayFoodPopAnimation();
    }

    public void Interact(PlayerController playerController)
    {
        if (playerController.IsCarry == true)
        {
            bool isAdded = AddIngredient(playerController.CurrentIngredientType);

            if (isAdded == true)
            {
                playerController.ClearIngredient();
                playerController.PlayCook();
            }

            return;
        }

        TakeFood(playerController);
    }

    private bool AddIngredient(IngredientType ingredientType)
    {
        if (_ingredientList.Count >= MAX_INGREDIENT_COUNT)
        {
            return false;
        }

        if (ingredientType == IngredientType.None)
        {
            return false;
        }

        if (_completedFoodType != IngredientType.None)
        {
            RuinFood();
        }

        _ingredientList.Add(ingredientType);

        RefreshIngredientIcons();

        CheckRecipe();

        return true;
    }

    private void RefreshIngredientIcons()
    {
        for (int i = 0; i < SpriteRenderer_IngredientIcons.Length; i++)
        {
            SpriteRenderer iconRenderer = SpriteRenderer_IngredientIcons[i];

            if (iconRenderer == null)
            {
                continue;
            }

            Sprite iconSprite = EmptyIcon;

            if (i < _ingredientList.Count)
            {
                iconSprite = ResourceManager.Inst.LoadSprite($"Icon/{_ingredientList[i]}");
            }

            if (iconSprite == null)
            {
                iconSprite = EmptyIcon;
            }

            iconRenderer.sprite = iconSprite;
            iconRenderer.enabled = true;
        }
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
        PlayCompleteEffect();
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

        RefreshIngredientIcons();
        RefreshFoodObject();
    }

    public void ResetStation()
    {
        if (_foodPopCoroutine != null)
        {
            StopCoroutine(_foodPopCoroutine);
            _foodPopCoroutine = null;
        }

        _ingredientList.Clear();

        RefreshIngredientIcons();
        
        _completedFoodType = IngredientType.None;

        if (_currentFoodObject == null)
        {
            return;
        }

        Destroy(_currentFoodObject);
        _currentFoodObject = null;
    }

    private void PlayCompleteEffect()
    {
        if (CompleteEffectPrefab == null)
        {
            return;
        }

        GameObject effectObject = Instantiate(CompleteEffectPrefab, FoodPoint.position, Quaternion.identity);

        Destroy(effectObject, 1f);
    }

    private void PlayFoodPopAnimation()
    {
        if (_currentFoodObject == null)
        {
            return;
        }

        if (_foodPopCoroutine != null)
        {
            StopCoroutine(_foodPopCoroutine);
        }

        _foodPopCoroutine = StartCoroutine(PlayFoodPopCoroutine(_currentFoodObject.transform));
    }

    private IEnumerator PlayFoodPopCoroutine(Transform targetTransform)
    {
        if (targetTransform == null)
        {
            yield break;
        }

        float elapsedTime = 0f;

        Vector3 startScale = Vector3.one * _foodPopStartScale;
        Vector3 endScale = Vector3.one * _foodPopEndScale;

        targetTransform.localScale = startScale;

        while (elapsedTime < _foodPopDuration)
        {
            if (targetTransform == null)
            {
                yield break;
            }

            elapsedTime += Time.deltaTime;

            float ratio = elapsedTime / _foodPopDuration;
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, ratio);

            yield return null;
        }

        elapsedTime = 0f;

        Vector3 finalScale = Vector3.one;

        while (elapsedTime < _foodPopDuration)
        {
            if (targetTransform == null)
            {
                yield break;
            }

            elapsedTime += Time.deltaTime;

            float ratio = elapsedTime / _foodPopDuration;
            targetTransform.localScale = Vector3.Lerp(endScale, finalScale, ratio);

            yield return null;
        }

        targetTransform.localScale = finalScale;
    }
}