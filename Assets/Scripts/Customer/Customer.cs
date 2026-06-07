using System;
using System.Collections;
using UnityEngine;

public class Customer : MonoBehaviour, IInteractable
{
    private enum CustomerState
    {
        MoveToWaitLine,
        WaitOrder,
        Exit
    }

    [SerializeField] private CustomerView CustomerView;

    [SerializeField] private IngredientType[] _orderFoodTypes;

    [SerializeField] private OrderBubbleUI OrderBubblePrefab;
    [SerializeField] private WorldTextPopup ScorePopupPrefab;

    [SerializeField] private SpriteRenderer SpriteRenderer_Reaction;

    [SerializeField] private Sprite Sprite_Success;
    [SerializeField] private Sprite Sprite_Wrong;
    [SerializeField] private Sprite Sprite_Timeout;

    [SerializeField] private AudioClip _serveSuccessSFX;

    [NonSerialized]
    private CustomerData _customerData;

    private IngredientType _orderFoodType;

    private Transform _orderBubbleLayout;
    private OrderBubbleUI _orderBubbleUI;

    private Vector3 _waitPosition;
    private Vector3 _exitPosition;
    private Vector2 _lastMoveDirection = Vector2.down;

    private float _moveSpeed;
    private float _waitTime;
    private int _rewardScore;
    private float _currentWaitTime;

    private CustomerState _customerState;

    private Action<Customer> _onExitCompleted;

    public IngredientType OrderFoodType
    {
        get
        {
            return _orderFoodType;
        }
    }

    private void Awake()
    {
        SpriteRenderer_Reaction.enabled = false;
    }

    private void Update()
    {
        if (GameManager.Inst.IsGamePlaying == false)
        {
            return;
        }

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

        RuntimeAnimatorController controller = ResourceManager.Inst.LoadAnimatorController(_customerData.AnimatorControllerPath);

        CustomerView.SetAnimatorController(controller);
    }

    public void Initialize(Transform orderBubbleLayout, Vector3 waitPosition, Vector3 exitPosition, Action<Customer> onExitCompleted)
    {
        _orderBubbleLayout = orderBubbleLayout;
        _waitPosition = waitPosition;
        _exitPosition = exitPosition;
        _onExitCompleted = onExitCompleted;

        _customerState = CustomerState.MoveToWaitLine;
        _currentWaitTime = _waitTime;
        _lastMoveDirection = Vector2.down;

        CustomerView.SetMove(true);
        CustomerView.SetDirection(_lastMoveDirection);
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
            ShowReaction(Sprite_Wrong);

            Debug.Log($"잘못된 음식입니다. 주문 : {_orderFoodType}");
            return;
        }

        ServeFood(playerController);
    }

    public void ClearRuntime()
    {
        GameManager.Inst.RemoveCustomerOrder(this);

        if (_orderBubbleUI == null)
        {
            return;
        }

        Destroy(_orderBubbleUI.gameObject);
        _orderBubbleUI = null;
    }

    private void MoveToWaitLine()
    {
        if (_customerState != CustomerState.MoveToWaitLine)
        {
            return;
        }

        Vector3 moveDirection = (_waitPosition - transform.position).normalized;

        RefreshMoveDirection(moveDirection);

        CustomerView.SetMove(true);

        transform.position = Vector3.MoveTowards(transform.position, _waitPosition, _moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _waitPosition);

        if (distance > 0.05f)
        {
            return;
        }

        _customerState = CustomerState.WaitOrder;
        _currentWaitTime = _waitTime;

        CustomerView.SetMove(false);
        CustomerView.SetDirection(_lastMoveDirection);

        InitializeOrderBubble();
    }

    private void MoveToExit()
    {
        if (_customerState != CustomerState.Exit)
        {
            return;
        }

        Vector3 moveDirection = (_exitPosition - transform.position).normalized;

        RefreshMoveDirection(moveDirection);

        CustomerView.SetMove(true);

        transform.position = Vector3.MoveTowards(transform.position, _exitPosition, _moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, _exitPosition);

        if (distance > 0.05f)
        {
            return;
        }

        _onExitCompleted?.Invoke(this);

        GameObjectManager.Inst.RemoveObject(gameObject);
    }

    private void HandleWaitTimer()
    {
        if (_customerState != CustomerState.WaitOrder)
        {
            return;
        }

        _currentWaitTime -= Time.deltaTime;

        RefreshWaitGauge();

        if (_currentWaitTime > 0)
        {
            return;
        }

        GameManager.Inst.AddFailedCustomer();

        ShowReaction(Sprite_Timeout);

        StartExit();
    }

    private void StartExit()
    {
        _customerState = CustomerState.Exit;

        Vector3 moveDirection = (_exitPosition - transform.position).normalized;

        RefreshMoveDirection(moveDirection);

        CustomerView.SetMove(true);

        ClearRuntime();
    }

    private void RefreshMoveDirection(Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero)
        {
            return;
        }

        _lastMoveDirection = new Vector2(moveDirection.x, moveDirection.y).normalized;

        CustomerView.SetDirection(_lastMoveDirection);
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
            GameManager.Inst.RefreshCustomerWaitRatio(this, 0f);
            return;
        }

        float waitRatio = Mathf.Clamp01(_currentWaitTime / _waitTime);

        _orderBubbleUI.SetWaitGauge(waitRatio);

        GameManager.Inst.RefreshCustomerWaitRatio(this, waitRatio);
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

        GameManager.Inst.AddCustomerOrder(this);
    }

    private void ServeFood(PlayerController playerController)
    {
        playerController.ClearIngredient();

        GameManager.Inst.AddScore(_rewardScore);

        CreateScorePopup();

        SoundManager.Inst.PlaySFX(_serveSuccessSFX);

        GameManager.Inst.AddCompletedOrder();

        ShowReaction(Sprite_Success);

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

    private void ShowReaction(Sprite reactionSprite)
    {
        if (SpriteRenderer_Reaction == null)
        {
            return;
        }

        if (reactionSprite == null)
        {
            return;
        }

        SpriteRenderer_Reaction.sprite = reactionSprite;
        SpriteRenderer_Reaction.enabled = true;

        StartCoroutine(PlayReactionCoroutine());
    }

    private IEnumerator PlayReactionCoroutine()
    {
        Transform reactionTransform = SpriteRenderer_Reaction.transform;

        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale = Vector3.one * 1.2f;

        float duration = 0.15f;
        float elapsedTime = 0f;

        reactionTransform.localScale = startScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float ratio = elapsedTime / duration;

            reactionTransform.localScale = Vector3.Lerp(startScale, endScale, ratio);

            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float ratio = elapsedTime / duration;

            reactionTransform.localScale = Vector3.Lerp(endScale, Vector3.one, ratio);

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        SpriteRenderer_Reaction.enabled = false;
    }
}