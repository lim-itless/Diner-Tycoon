using UnityEngine;

public class Customer : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientType _orderFoodType;
    [SerializeField] private IngredientType[] _orderFoodTypes;
    [SerializeField] private int _rewardScore = 100;
    [SerializeField] private OrderBubbleUI OrderBubbleUI;

    private void Start()
    {
        SetRandomOrder();
        InitializeOrderBubble();
    }

    private void SetRandomOrder()
    {
        if (_orderFoodTypes == null || _orderFoodTypes.Length == 0)
        {
            _orderFoodType = IngredientType.ClamChowder;
            return;
        }

        int randomIndex = Random.Range(0, _orderFoodTypes.Length);

        _orderFoodType = _orderFoodTypes[randomIndex];
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