using UnityEngine;

public class Customer : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientType _orderFoodType = IngredientType.ClamChowder;
    [SerializeField] private int _rewardScore = 100;
    [SerializeField] private OrderBubbleUI OrderBubbleUI;

    private void Start()
    {
        InitializeOrderBubble();
    }

    private void InitializeOrderBubble()
    {
        OrderBubbleUI.Initialize(this.transform,_orderFoodType.ToString());
    }

    public void Interact(PlayerController playerController)
    {
        if (playerController.IsCarry == false)
        {
            Debug.Log("손님에게 줄 음식이 없습니다.");
            return;
        }

        if (playerController.CurrentIngredientType != _orderFoodType)
        {
            Debug.Log($"잘못된 음식입니다. 주문 : {_orderFoodType}");
            return;
        }

        ServeFood(playerController);
    }

    private void ServeFood(PlayerController playerController)
    {
        playerController.ClearIngredient();

        GameManager.Inst.AddScore(_rewardScore);
    }
}