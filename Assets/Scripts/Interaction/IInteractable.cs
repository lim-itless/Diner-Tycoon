using UnityEngine;

public enum InteractableType
{
    None,
    IngredientBox,
    CookStation,
    Customer
}

public interface IInteractable
{
    InteractableType InteractableType { get; }

    void Interact(PlayerController playerController);
}
