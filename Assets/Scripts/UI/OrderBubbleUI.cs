using TMPro;
using UnityEngine;

public class OrderBubbleUI : UIBase
{
    [SerializeField] private TMP_Text Text_Order;

    private Transform _targetTransform;
    private Camera _mainCamera;

    public void Initialize(Transform targetTransform, string orderText)
    {
        _targetTransform = targetTransform;

        Text_Order.text = $"{orderText} !";

        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        RefreshPosition();
    }

    private void RefreshPosition()
    {
        if (_targetTransform == null)
        {
            return;
        }

        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(_targetTransform.position + Vector3.up * 1f);

        transform.position = screenPosition;
    }
}