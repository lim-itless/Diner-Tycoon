using UnityEngine;

public class CustomerView : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private static readonly int _isMoveHash = Animator.StringToHash("IsMove");
    private static readonly int _dirXHash = Animator.StringToHash("DirX");
    private static readonly int _dirYHash = Animator.StringToHash("DirY");

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.runtimeAnimatorController = controller;
    }

    public void SetMove(bool isMove)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool(_isMoveHash, isMove);
    }

    public void SetDirection(Vector2 direction)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetFloat(_dirXHash, Mathf.Abs(direction.x));
        _animator.SetFloat(_dirYHash, direction.y);

        FlipByDirection(direction);
    }

    private void FlipByDirection(Vector2 direction)
    {
        if (_spriteRenderer == null)
        {
            return;
        }

        if (Mathf.Abs(direction.x) <= 0.01f)
        {
            return;
        }

        _spriteRenderer.flipX = direction.x < 0f;
    }
}