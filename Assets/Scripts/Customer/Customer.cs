using System;
using UnityEngine;

public class Customer : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientType _orderFoodType;
    [SerializeField] private IngredientType[] _orderFoodTypes;
    [SerializeField] private int _rewardScore = 100;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _waitTime = 10f;

    [SerializeField] private OrderBubbleUI OrderBubblePrefab;
    [SerializeField] private WorldTextPopup ScorePopupPrefab;

    [SerializeField] private AudioClip _serveSuccessSFX;

    private Transform OrderBubble_Layout;
    private OrderBubbleUI _orderBubbleUI;

    private Vector3 _waitPosition;
    private Vector3 _exitPosition;

    private bool _isMovingToWaitLine;
    private bool _isMovingToExit;
    private bool _isReadyToOrder;
    private float _currentWaitTime;

    private Action<Customer> _onExitCompleted;

    private void Start()
    {
        SetRandomOrder();
    }
    private void Update()
    {
        MoveToWaitLine();
        MoveToExit();
        HandleWaitTimer();
    }

    public void Initialize(Transform orderBubbleLayout, Vector3 waitPosition, Vector3 exitPosition, Action<Customer> onExitCompleted)
    {
        OrderBubble_Layout = orderBubbleLayout;
        _waitPosition = waitPosition;
        _exitPosition = exitPosition;
        _onExitCompleted = onExitCompleted;

        _isMovingToWaitLine = true;
        _isMovingToExit = false;
        _isReadyToOrder = false;
    }

    private void MoveToWaitLine()
    {
        if (_isMovingToWaitLine == false)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, _waitPosition, _moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _waitPosition);
        if (distance > 0.05f)
        {
            return;
        }

        _isMovingToWaitLine = false;
        _isReadyToOrder = true;
        _currentWaitTime = _waitTime;

        InitializeOrderBubble();
    }

    private void MoveToExit()
    {
        if (_isMovingToExit == false)
        { 
            return ;
        }

        transform.position = Vector3.MoveTowards(transform.position, _exitPosition, _moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _exitPosition);
        if (distance > 0.05f)
        {
            return;
        }

        _onExitCompleted?.Invoke(this);

        Destroy(this.gameObject);

    }

    private void HandleWaitTimer()
    { 
        if(_isReadyToOrder ==  false)
        {
            return;
        }

        _currentWaitTime -= Time.deltaTime;

        if (_orderBubbleUI != null)
        {
            _orderBubbleUI.SetWaitGauge(_currentWaitTime / _waitTime);
        }

        if (_currentWaitTime > 0)
        {
            return;
        }

        StartExit();
    }

    private void StartExit()
    {
        _isMovingToWaitLine = false;
        _isMovingToExit = true;
        _isReadyToOrder = false;

        if (_orderBubbleUI != null)
        {
            Destroy(_orderBubbleUI.gameObject);
        }
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

        _orderBubbleUI = Instantiate(OrderBubblePrefab, OrderBubble_Layout);
        _orderBubbleUI.transform.localScale = Vector3.one;
        _orderBubbleUI.Initialize(this.transform, _orderFoodType);
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

        CreateScorePopup();
        SoundManager.Inst.PlaySFX(_serveSuccessSFX);

        Debug.Log($"{_orderFoodType} 전달 완료");
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
}