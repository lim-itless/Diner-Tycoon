using UnityEngine;

public class IngredientBox : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientType _ingredientType;

    public void Interact(PlayerController playerController)
    {
        playerController.CarryIngredient(_ingredientType);
    }
}
