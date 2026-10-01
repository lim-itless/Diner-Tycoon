using UnityEngine;

public class TutorialPointer : MonoBehaviour
{
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1f, 0f);

    private Transform _target;

    private void LateUpdate()
    {
        RefreshPosition();
    }

    public void SetTarget(Transform target)
    {
        _target = target;

        if (_target == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);
        RefreshPosition();
    }

    public void Hide()
    {
        _target = null;
        gameObject.SetActive(false);
    }

    private void RefreshPosition()
    {
        if (_target == null)
        {
            return;
        }

        transform.position = _target.position;
    }
}