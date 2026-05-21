using UnityEngine;

public class Customer : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientType _orderFoodType;
    [SerializeField] private IngredientType[] _orderFoodTypes;
    [SerializeField] private int _rewardScore = 100;
    [SerializeField] private float _moveSpeed = 2f;

    [SerializeField] private OrderBubbleUI OrderBubblePrefab;
    
    private Transform OrderBubble_Layout;
    private OrderBubbleUI _orderBubbleUI;
    private Vector3 _targetPosition;
    private bool _isMoving;
    private bool _isReadyToOrder;

    private void Start()
    {
        SetRandomOrder();
    }
    private void Update()
    {
        MoveToTarget();
    }

    public void MoveToQueueSpot(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        _isMoving = true;
        _isReadyToOrder = false;
    }

    private void MoveToTarget()
    {
        if (_isMoving == false)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _targetPosition);

        if (distance > 0.05f)
        {
            return;
        }

        _isMoving = false;
        _isReadyToOrder = true;

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

    public void Initialize(Transform orderBubbleLayout)
    {
        OrderBubble_Layout = orderBubbleLayout;
    }

    private void InitializeOrderBubble()
    {
        if (_orderBubbleUI != null)
        {
            return;
        }

        _orderBubbleUI = Instantiate(OrderBubblePrefab, OrderBubble_Layout);

        _orderBubbleUI.transform.localScale = Vector3.one;

        _orderBubbleUI.Initialize(this.transform, _orderFoodType.ToString());
    }

    public void Interact(PlayerController playerController)
    {
        if (_isReadyToOrder == false)
        {
            Debug.Log("아직 대기열에 도착하지 않았습니다.");
            return;
        }

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

        if (_orderBubbleUI != null)
        {
            Destroy(_orderBubbleUI.gameObject);
        }

        Destroy(gameObject);

        Debug.Log($"{_orderFoodType} 전달 완료");
    }
}