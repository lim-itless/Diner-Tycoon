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

    private GameObject _currentCarryObject;
    private Vector2 _moveInput;
    private Vector2 _lastMoveDirection = Vector2.down;
    private float _lastHorizontalDirection = 1f;
    public IngredientType CurrentIngredientType { get; private set; }
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
        RefreshPlayerView();
    }

    private void FixedUpdate()
    {
        Move();
        //Dance();
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

    private void RefreshPlayerView()
    {
        bool isMove = _moveInput != Vector2.zero;

        PlayerView.SetMove(isMove);
        PlayerView.SetDirection(_lastMoveDirection);
        PlayerView.Flip(_lastHorizontalDirection);
    }

    private void Move()
    {
        PlayerRigidbody.linearVelocity = _moveInput * _moveSpeed;
    }

    private void HandleInteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E) == false)
        {
            return;
        }

        TryInteract();
    }

    private void TryInteract()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, _interactRange,_interactableLayer);

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
        PlayerView.SetCarry(true);
    }

    public void ClearIngredient()
    {
        CurrentIngredientType = IngredientType.None;
        IsCarry = false;

        ClearCarryObject();
        PlayerView.SetCarry(false);
    }

    //private void Dance()
    //{
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        PlayerView.SetDance(true);
    //        Invoke(nameof(EndDance), 0.5f);
    //    }
    //}

    //private void EndDance()
    //{
    //    PlayerView.SetDance(false);
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, _interactRange);
    }
        private void CreateCarryObject(IngredientType ingredientType)
    {
        ClearCarryObject();

        GameObject carryPrefab = Resources.Load<GameObject>($"Prefabs/Food/Food_{ingredientType}");

        if (carryPrefab == null)
        {
            return;
        }

        _currentCarryObject = Instantiate(carryPrefab, HoldSpot);
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

}