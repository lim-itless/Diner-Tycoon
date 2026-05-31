using System;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Customer : MonoBehaviour, IInteractable
{
    private enum CustomerState
    {
        MoveToWaitLine,
        WaitOrder,
        Exit
    }

    public enum CustomerFaceState
    {
        Normal,
        Bad,
        Angry
    }

    public enum CustomerDirection
    {
        Front,
        Side,
        Back
    }

    [SerializeField] private SpriteRenderer BodyRenderer;
    [SerializeField] private SpriteRenderer CustomerRenderer;
    [SerializeField] private CustomerView CustomerView;

    [SerializeField] private IngredientType[] _orderFoodTypes;

    [SerializeField] private OrderBubbleUI OrderBubblePrefab;
    [SerializeField] private WorldTextPopup ScorePopupPrefab;

    [SerializeField] private AudioClip _serveSuccessSFX;

    [NonSerialized]
    private CustomerData _customerData;
    private IngredientType _orderFoodType;

    private Transform _orderBubbleLayout;
    private OrderBubbleUI _orderBubbleUI;

    private Vector3 _waitPosition;
    private Vector3 _exitPosition;

    private float _moveSpeed;
    private float _waitTime;
    private int _rewardScore;
    private float _currentWaitTime;

    private CustomerState _customerState;
    private CustomerFaceState _faceState;
    private CustomerDirection _customerDirection;

    private Action<Customer> _onExitCompleted;

    private void Update()
    {
        MoveToWaitLine();
        MoveToExit();
        HandleWaitTimer();
    }

    public void InitializeData(CustomerData customerData)
    {
        if (customerData == null)
        {
            Debug.LogWarning("CustomerData가 null 입니다.");
            return;
        }

        _customerData = customerData;

        _waitTime = customerData.WaitTime;
        _moveSpeed = customerData.MoveSpeed;
        _rewardScore = customerData.RewardScore;

        SetRandomOrder();
        
        CustomerType customerType = customerData.GetCustomerType();

        switch (customerType)
        {
            case CustomerType.Fast:
                _waitTime *= 0.7f;
                break;

            case CustomerType.VIP:
                _rewardScore *= 2;
                break;
        }

        _faceState = CustomerFaceState.Normal;
        _customerDirection = CustomerDirection.Front;

        RuntimeAnimatorController controller = ResourceManager.Inst.LoadAnimatorController(_customerData.AnimatorControllerPath);
        CustomerView.SetAnimatorController(controller);

        RefreshView();
    }

    public void Initialize(Transform orderBubbleLayout, Vector3 waitPosition, Vector3 exitPosition, Action<Customer> onExitCompleted)
    {
        _orderBubbleLayout = orderBubbleLayout;
        _waitPosition = waitPosition;
        _exitPosition = exitPosition;
        _onExitCompleted = onExitCompleted;

        _customerState = CustomerState.MoveToWaitLine;

        CustomerView.PlayAnimation(CustomerAnimAction.Walk);
    }

    public void Interact(PlayerController playerController)
    {
        if (_customerState != CustomerState.WaitOrder)
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

    private void MoveToWaitLine()
    {
        if (_customerState != CustomerState.MoveToWaitLine)
        {
            return;
        }

        Vector3 moveDirection = (_waitPosition - transform.position).normalized;

        RefreshDirection(moveDirection);
        RefreshView();

        transform.position = Vector3.MoveTowards(transform.position, _waitPosition, _moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _waitPosition);

        if (distance > 0.05f)
        {
            return;
        }

        _customerState = CustomerState.WaitOrder;
        _currentWaitTime = _waitTime;

        CustomerView.PlayAnimation(CustomerAnimAction.Idle);

        InitializeOrderBubble();
    }

    private void MoveToExit()
    {
        if (_customerState != CustomerState.Exit)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _exitPosition, _moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _exitPosition);

        if (distance > 0.05f)
        {
            return;
        }

        _onExitCompleted?.Invoke(this);

        Destroy(gameObject);
    }

    private void HandleWaitTimer()
    {
        if (_customerState != CustomerState.WaitOrder)
        {
            return;
        }

        _currentWaitTime -= Time.deltaTime;

        RefreshWaitGauge();
        RefreshFaceState();

        if (_currentWaitTime > 0)
        {
            return;
        }

        GameManager.Inst.AddFailedCustomer();
        StartExit();
    }

    private void RefreshWaitGauge()
    {
        if (_orderBubbleUI == null)
        {
            return;
        }

        if (_waitTime <= 0)
        {
            _orderBubbleUI.SetWaitGauge(0f);
            return;
        }

        _orderBubbleUI.SetWaitGauge(_currentWaitTime / _waitTime);
    }

    private void StartExit()
    {
        _customerState = CustomerState.Exit;

        CustomerView.PlayAnimation(CustomerAnimAction.Walk);

        if (_orderBubbleUI == null)
        {
            return;
        }

        GameObjectManager.Inst.RemoveObject(_orderBubbleUI.gameObject);
    }

    private void SetRandomOrder()
    {
        if (_orderFoodTypes == null || _orderFoodTypes.Length == 0)
        {
            _orderFoodType = IngredientType.ClamChowder;
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, _orderFoodTypes.Length);

        _orderFoodType = _orderFoodTypes[randomIndex];
    }

    private void InitializeOrderBubble()
    {
        if (_orderBubbleUI != null)
        {
            return;
        }

        if (OrderBubblePrefab == null || _orderBubbleLayout == null)
        {
            return;
        }

        _orderBubbleUI = Instantiate(OrderBubblePrefab, _orderBubbleLayout);

        _orderBubbleUI.transform.localScale = Vector3.one;
        _orderBubbleUI.Initialize(transform, _orderFoodType);
    }

    private void ServeFood(PlayerController playerController)
    {
        playerController.ClearIngredient();

        GameManager.Inst.AddScore(_rewardScore);
        CreateScorePopup();
        SoundManager.Inst.PlaySFX(_serveSuccessSFX);
        GameManager.Inst.AddCompletedOrder();

        StartExit();
    }

    private void CreateScorePopup()
    {
        if (ScorePopupPrefab == null)
        {
            return;
        }

        WorldTextPopup popup = Instantiate(ScorePopupPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);

        popup.SetText($"+ {_rewardScore}");
    }

    private void RefreshFaceState()
    {
        if (_waitTime <= 0)
        {
            return;
        }

        float waitRatio = _currentWaitTime / _waitTime;

        if (waitRatio <= 0.3f)
        {
            ChangeFaceState(CustomerFaceState.Angry);
            return;
        }

        if (waitRatio <= 0.6f)
        {
            ChangeFaceState(CustomerFaceState.Bad);
            return;
        }

        ChangeFaceState(CustomerFaceState.Normal);
    }

    private void ChangeFaceState(CustomerFaceState faceState)
    {
        if (_faceState == faceState)
        {
            return;
        }

        _faceState = faceState;

        RefreshView();
    }

    private void RefreshView()
    {
        if (CustomerRenderer == null)
        {
            return;
        }

        string spriteId = GetCurrentSpriteId();

        if (string.IsNullOrEmpty(spriteId) == true)
        {
            return;
        }

        Sprite sprite = ResourceManager.Inst.LoadSprite(spriteId);

        if (sprite == null)
        {
            return;
        }

        CustomerRenderer.sprite = sprite;
    }

    private string GetCurrentSpriteId()
    {
        //if (_customerData == null)
        //{
        //    return string.Empty;
        //}

        //if (_faceState == CustomerFaceState.Angry)
        //{
        //    if (_customerDirection == CustomerDirection.Back)
        //    {
        //        return _customerData.AngryBackSpriteId;
        //    }

        //    if (_customerDirection == CustomerDirection.Side)
        //    {
        //        return _customerData.AngrySideSpriteId;
        //    }

        //    return _customerData.AngryFrontSpriteId;
        //}

        //if (_faceState == CustomerFaceState.Bad)
        //{
        //    if (_customerDirection == CustomerDirection.Back)
        //    {
        //        return _customerData.BadBackSpriteId;
        //    }

        //    if (_customerDirection == CustomerDirection.Side)
        //    {
        //        return _customerData.BadSideSpriteId;
        //    }

        //    return _customerData.BadFrontSpriteId;
        //}

        //if (_customerDirection == CustomerDirection.Back)
        //{
        //    return _customerData.NormalBackSpriteId;
        //}

        //if (_customerDirection == CustomerDirection.Side)
        //{
        //    return _customerData.NormalSideSpriteId;
        //}

        return "";
    }

    private void RefreshDirection(Vector3 moveDirection)
    {
        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            _customerDirection = CustomerDirection.Side;
            return;
        }

        if (moveDirection.y > 0)
        {
            _customerDirection = CustomerDirection.Back;
            return;
        }

        _customerDirection = CustomerDirection.Front;
    }
}