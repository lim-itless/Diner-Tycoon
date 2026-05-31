using UnityEngine;

public class CustomerView : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void PlayAnimation(CustomerAnimAction action)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.Play(action.ToString());
    }

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        if (_animator == null)
        {
            return;
        }

        _animator.runtimeAnimatorController = controller;
    }
}