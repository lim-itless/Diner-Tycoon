using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerView PlayerView;
    [SerializeField] private Rigidbody2D PlayerRigidbody;

    [SerializeField] private float _moveSpeed = 5f;
    // [TODO] 대쉬를 넣으면 좋을 것 같음 (Shift)

    [SerializeField] private float _interactRange = 1.5f;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private Transform HoldSpot;
    [SerializeField] private Vector2 _interactOffset = new Vector2(0f, 4f);

    private GameObject _currentCarryObject;
    private Vector2 _moveInput;
    private Vector2 _lastMoveDirection = Vector2.down;
    private float _lastHorizontalDirection = 1f;
    private bool _isCook;
    private bool _isDance;

    public IngredientType CurrentIngredientType { get; private set; }
    public PlayerState CurrentPlayerState { get; private set; }
    public bool IsCarry { get; private set; }

    private void Awake()
    {
        PlayerRigidbody.gravityScale = 0f;
        PlayerRigidbody.freezeRotation = true;
    }

    private void Update()
    {
        ReadMoveInput();
        HandleInteractInput();
        HandleDanceInput();
        RefreshPlayerState();
        RefreshPlayerView();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ReadMoveInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        _moveInput = new Vector2(moveX, moveY).normalized;

        if (_moveInput != Vector2.zero)
        {
            _lastMoveDirection = _moveInput;
        }

        if (moveX != 0)
        {
            _lastHorizontalDirection = moveX;
        }
    }

    private void HandleInteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E) == false)
        {
            return;
        }

        TryInteract();
    }

    private void HandleDanceInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) == false)
        {
            return;
        }

        PlayDance();
    }

    private void RefreshPlayerState()
    {
        bool isMove = _moveInput != Vector2.zero;

        if (IsCarry == true)
        {
            CurrentPlayerState = isMove == true ? PlayerState.CarryMove : PlayerState.CarryIdle;
            return;
        }

        CurrentPlayerState = isMove == true ? PlayerState.Move : PlayerState.Idle;
    }

    private void RefreshPlayerView()
    {
        bool isMove = CurrentPlayerState == PlayerState.Move || CurrentPlayerState == PlayerState.CarryMove;
        bool isCarry = CurrentPlayerState == PlayerState.CarryIdle || CurrentPlayerState == PlayerState.CarryMove;
        bool isCook = _isCook;
        bool isDance = _isDance;

        PlayerView.SetMove(isMove);
        PlayerView.SetCarry(isCarry);
        PlayerView.SetCook(isCook);
        PlayerView.SetDance(isDance);

        PlayerView.SetDirection(_lastMoveDirection);
        PlayerView.Flip(_lastHorizontalDirection);
    }

    public void PlayCook()
    {
        _isCook = true;

        RefreshPlayerView();

        CancelInvoke(nameof(StopCook));
        Invoke(nameof(StopCook), 0.4f);
    }

    private void StopCook()
    {
        _isCook = false;

        RefreshPlayerView();
    }

    private void PlayDance()
    {
        _isDance = true;

        RefreshPlayerView();

        CancelInvoke(nameof(StopDance));
        Invoke(nameof(StopDance), 0.6f);
    }

    private void StopDance()
    {
        _isDance = false;

        RefreshPlayerView();
    }

    private void Move()
    {
        PlayerRigidbody.linearVelocity = _moveInput * _moveSpeed;
    }

    private void TryInteract()
    {
        Vector2 interactCenter = (Vector2)transform.position + _interactOffset;

        Collider2D hitCollider = Physics2D.OverlapCircle(interactCenter, _interactRange,_interactableLayer);

        if (hitCollider == null)
        {
            return;
        }

        IInteractable interactable = hitCollider.GetComponent<IInteractable>();

        if (interactable == null)
        {
            return;
        }

        interactable.Interact(this);
    }

    public void CarryIngredient(IngredientType ingredientType)
    {
        if (IsCarry == true)
        {
            return;
        }

        CurrentIngredientType = ingredientType;
        IsCarry = true;

        CreateCarryObject(ingredientType);
    }

    public void ClearIngredient()
    {
        CurrentIngredientType = IngredientType.None;
        IsCarry = false;

        ClearCarryObject();

        RefreshPlayerState();
        RefreshPlayerView();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere((Vector2)transform.position + _interactOffset, _interactRange);
    }

    private void CreateCarryObject(IngredientType ingredientType)
    {
        ClearCarryObject();

        GameObject carryPrefab = ResourceManager.Inst.LoadPrefab($"Prefabs/Food/Food_{ingredientType}");

        if (carryPrefab == null)
        {
            Debug.LogWarning($"Carry Prefab 없음 : Food_{ingredientType}");
            return;
        }

        _currentCarryObject = Instantiate(carryPrefab, HoldSpot);
        _currentCarryObject.transform.localPosition = Vector3.zero;
    }

    private void ClearCarryObject()
    {
        if (_currentCarryObject == null)
        {
            return;
        }

        Destroy(_currentCarryObject);
        _currentCarryObject = null;
    }

    public void ResetPlayerState()
    {
        ClearIngredient();

        _moveInput = Vector2.zero;
        CurrentPlayerState = PlayerState.Idle;

        RefreshPlayerView();
    }
}