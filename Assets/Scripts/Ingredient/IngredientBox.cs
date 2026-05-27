using UnityEngine;

public class IngredientBox : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientType _ingredientType;
    [SerializeField] private AudioClip _pickupSFX;

    public void Interact(PlayerController playerController)
    {
        playerController.CarryIngredient(_ingredientType);
        SoundManager.Inst.PlaySFX(_pickupSFX);
    }
}
