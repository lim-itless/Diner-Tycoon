using UnityEngine;

public class Customer : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientType _orderFoodType = IngredientType.ClamChowder;
    [SerializeField] private int _rewardScore = 100;

    public void Interact(PlayerController playerController)
    {
        if (playerController.IsCarry == false)
        {
            Debug.Log("손님에게 줄 음식이 없습니다.");
            return;
        }

        if (playerController.CurrentIngredientType != _orderFoodType)
        {
            Debug.Log($"잘못된 음식입니다. 주문: {_orderFoodType}, 보유: {playerController.CurrentIngredientType}");
            return;
        }

        ServeFood(playerController);
    }

    private void ServeFood(PlayerController playerController)
    {
        playerController.ClearIngredient();

        Debug.Log($"{_orderFoodType} 서빙 성공!");
        GameManager.Instance.AddScore(_rewardScore);
    }
}