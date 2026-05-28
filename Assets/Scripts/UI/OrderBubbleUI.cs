using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderBubbleUI : UIBase
{
    [SerializeField] private TMP_Text Text_Order;
    [SerializeField] private Image Image_WaitGauge;
    [SerializeField] private Image Image_OrderIcon;

    [SerializeField] private Color _dangerColor = Color.red;
    [SerializeField] private Color _normalColor = Color.green;

    private Transform _targetTransform;
    private Camera _mainCamera;

    public void Initialize(Transform targetTransform, IngredientType orderType)
    {
        _targetTransform = targetTransform;

        Text_Order.text = string.Empty;

        RefreshOrderIcon(orderType);

        _mainCamera = Camera.main;

        SetWaitGauge(1f);
    }

    public void SetWaitGauge(float ratio)
    {
        ratio = Mathf.Clamp01(ratio);

        if (Image_WaitGauge != null)
        {
            Image_WaitGauge.fillAmount = ratio;
        }

        RefreshBubbleColor(ratio);
    }

    private void RefreshBubbleColor(float ratio)
    {
        if (Image_WaitGauge == null)
        {
            return;
        }

        Image_WaitGauge.color = Color.Lerp(_dangerColor, _normalColor, ratio);
    }

    private void RefreshOrderIcon(IngredientType orderType)
    {
        if (Image_OrderIcon == null)
        {
            return;
        }

        Sprite iconSprite = ResourceManager.Inst.LoadSprite($"Icon/{orderType}");

        if (iconSprite == null)
        {
            return;
        }

        Image_OrderIcon.sprite = iconSprite;
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

        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(_targetTransform.position + Vector3.up * 2f);

        transform.position = screenPosition;
    }
}