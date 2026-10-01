using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Inst { get; private set; }

    [SerializeField] private Transform Target_OrderSlotAnchor;
    [SerializeField] private Transform Target_IngredientAnchor;
    [SerializeField] private Transform Target_CookStationAnchor;
    [SerializeField] private Transform Target_CustomerAnchor;
    [SerializeField] private TutorialPointer TutorialPointer;

    public event Action<TutorialStep> OnTutorialStepChanged;

    public TutorialStep CurrentStep { get; private set; }

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
    }

    private void Update()
    {
        if (SaveManager.Inst.SaveData.IsTutorialCompleted == true)
        {
            return;
        }

        if (CurrentStep != TutorialStep.CheckRecipe)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) == false)
        {
            return;
        }

        SetStep(TutorialStep.PickIngredient);
    }

    public void StartTutorial()
    {
        SetStep(TutorialStep.CheckRecipe);
    }

    public void SetCustomerTarget(Transform customerTransform)
    {
        Target_CustomerAnchor = customerTransform;

        if (CurrentStep == TutorialStep.ServeFood)
        {
            TutorialPointer.SetTarget(Target_CustomerAnchor);
        }
    }

    public bool IsInteractableAllowed(IInteractable interactable)
    {
        if (SaveManager.Inst.SaveData.IsTutorialCompleted == true)
        {
            return true;
        }

        if (interactable == null)
        {
            return false;
        }

        switch (CurrentStep)
        {
            case TutorialStep.CheckRecipe:
                return false;

            case TutorialStep.PickIngredient:
                return interactable.InteractableType == InteractableType.IngredientBox;

            case TutorialStep.CookFood:
                return interactable.InteractableType == InteractableType.CookStation;

            case TutorialStep.ServeFood:
                return interactable.InteractableType == InteractableType.Customer;
        }

        return false;
    }

    public void NotifyInteractSuccess(InteractableType interactableType)
    {
        if (SaveManager.Inst.SaveData.IsTutorialCompleted == true)
        {
            return;
        }

        if (CurrentStep == TutorialStep.PickIngredient && interactableType == InteractableType.IngredientBox)
        {
            SetStep(TutorialStep.CookFood);
            return;
        }

        if (CurrentStep == TutorialStep.CookFood && interactableType == InteractableType.CookStation)
        {
            SetStep(TutorialStep.ServeFood);
            return;
        }

        if (CurrentStep == TutorialStep.ServeFood && interactableType == InteractableType.Customer)
        {
            SetStep(TutorialStep.Complete);
        }
    }

    private void SetStep(TutorialStep step)
    {
        CurrentStep = step;

        OnTutorialStepChanged?.Invoke(CurrentStep);

        switch (CurrentStep)
        {
            case TutorialStep.CheckRecipe:
                TutorialPointer.SetTarget(Target_OrderSlotAnchor);
                break;

            case TutorialStep.PickIngredient:
                TutorialPointer.SetTarget(Target_IngredientAnchor);
                break;

            case TutorialStep.CookFood:
                TutorialPointer.SetTarget(Target_CookStationAnchor);
                break;

            case TutorialStep.ServeFood:
                TutorialPointer.SetTarget(Target_CustomerAnchor);
                break;

            case TutorialStep.Complete:
                TutorialPointer.Hide();
                break;
        }
    }
}