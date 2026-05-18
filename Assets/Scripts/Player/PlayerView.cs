using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator Animator_Player;
    [SerializeField] private SpriteRenderer SpriteRenderer_Player;

    // AI 추천...why?
    private static readonly int _isMoveHash = Animator.StringToHash("IsMove");
    private static readonly int _isCarryHash = Animator.StringToHash("IsCarry");
    private static readonly int _isCookHash = Animator.StringToHash("IsCook");
    private static readonly int _isServeHash = Animator.StringToHash("IsServe");

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

    public void SetServe(bool isServe)
    {
        Animator_Player.SetBool(_isServeHash, isServe);
    }


    //public void SetDance(bool isDance)
    //{
    //    animator.SetBool("IsDance", isDance);
    //}

    public void Flip(float moveX)
    {
        if (moveX == 0)
        {
            return;
        }

       SpriteRenderer_Player.flipX = moveX < 0;
    }
}