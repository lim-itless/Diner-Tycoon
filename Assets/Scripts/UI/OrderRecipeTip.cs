using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderRecipeTip : UIBase
{
    [SerializeField] private Image[] Image_IngredientIcons;



    public void Open(List<string> ingredientNames, Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        RefreshIngredientIcons(ingredientNames);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    private void RefreshIngredientIcons(List<string> ingredientNames)
    {
        for (int i = 0; i < Image_IngredientIcons.Length; i++)
        {
            if (Image_IngredientIcons[i] == null)
            {
                continue;
            }

            if (ingredientNames == null || i >= ingredientNames.Count)
            {
                Image_IngredientIcons[i].gameObject.SetActive(false);
                continue;
            }

            Sprite iconSprite = ResourceManager.Inst.LoadSprite($"Icon/{ingredientNames[i]}");

            if (iconSprite == null)
            {
                Image_IngredientIcons[i].gameObject.SetActive(false);
                continue;
            }

            Image_IngredientIcons[i].sprite = iconSprite;
            Image_IngredientIcons[i].gameObject.SetActive(true);
        }
    }
}