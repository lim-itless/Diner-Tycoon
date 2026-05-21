using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator Animator_Player;
    [SerializeField] private SpriteRenderer SpriteRenderer_Player;

    private static readonly int _isMoveHash = Animator.StringToHash("IsMove");
    private static readonly int _isCarryHash = Animator.StringToHash("IsCarry");
    private static readonly int _isCookHash = Animator.StringToHash("IsCook");
    private static readonly int _dirXHash = Animator.StringToHash("DirX");
    private static readonly int _dirYHash = Animator.StringToHash("DirY");

    public void SetMove(bool isMove)
    {
        Animator_Player.SetBool(_isMoveHash, isMove);
    }

    public void SetCarry(bool isCarry)
    {
        Animator_Player.SetBool(_isCarryHash, isCarry);
    }
    public void SetCook(bool isCook)
    {
        Animator_Player.SetBool(_isCookHash, isCook);
    }

    //public void SetDance(bool isDance)
    //{
    //    animator.SetBool("IsDance", isDance);
    //}

    public void SetDirection(Vector2 direction)
    {
        Animator_Player.SetFloat(_dirXHash, direction.x);
        Animator_Player.SetFloat(_dirYHash, direction.y);
    }

    public void Flip(float moveX)
    {
        if (moveX == 0)
        {
            return;
        }

       SpriteRenderer_Player.flipX = moveX < 0;
    }
}