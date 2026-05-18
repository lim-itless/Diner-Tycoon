using UnityEngine;

public class IngredientBox : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("재료 상자 터치!");
    }
}
