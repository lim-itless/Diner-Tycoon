using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrderSlotUI : UIBase, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image Image_OrderIcon;
    [SerializeField] private Image Image_WaitGauge;

    private IngredientType _orderFoodType;
    private OrderRecipeTip _tooltip;

    public bool IsUsing { get; private set; }

    public void Initialize(OrderRecipeTip tooltip)
    {
        _tooltip = tooltip;
        Close();
    }

    public void Open(IngredientType orderFoodType)
    {
        _orderFoodType = orderFoodType;
        IsUsing = true;
        gameObject.SetActive(true);

        RefreshOrderIcon(orderFoodType);
        RefreshWaitGauge(1f);
    }

    public override void Close()
    {
        IsUsing = false;
        gameObject.SetActive(false);

        if (_tooltip != null)
        {
            _tooltip.Close();
        }
    }

    public void RefreshWaitGauge(float waitRatio)
    {
        waitRatio = Mathf.Clamp01(waitRatio);

        if (Image_WaitGauge == null)
        {
            return;
        }

        Image_WaitGauge.fillAmount = waitRatio;

        Color waitColor;

        if (waitRatio > 0.5f)
        {
            waitColor = Color.Lerp(Color.yellow, Color.green, (waitRatio - 0.5f) / 0.5f);
        }
        else
        {
            waitColor = Color.Lerp(Color.red, Color.yellow, waitRatio / 0.5f);
        }

        Image_WaitGauge.color = waitColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltip == null)
        {
            return;
        }

        RecipeData recipeData = GameDataManager.Inst.GetRecipeData(_orderFoodType);

        if (recipeData == null)
        {
            return;
        }

        _tooltip.Open(recipeData.Ingredients, Input.mousePosition + new Vector3(80f, -40f, 0f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltip == null)
        {
            return;
        }

        _tooltip.Close();
    }

    private void RefreshOrderIcon(IngredientType orderFoodType)
    {
        if (Image_OrderIcon == null)
        {
            return;
        }

        Sprite iconSprite = ResourceManager.Inst.LoadSprite($"Icon/{orderFoodType}");

        if (iconSprite == null)
        {
            return;
        }

        Image_OrderIcon.sprite = iconSprite;
    }
}