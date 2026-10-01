using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainHUD : UIBase
{
    [SerializeField] private TMP_Text Text_TutorialGuide;
    [SerializeField] private TMP_Text Text_Day;
    [SerializeField] private TMP_Text Text_Time;
    [SerializeField] private TMP_Text Text_Score;
    [SerializeField] private OrderSlotUI[] OrderSlots;
    [SerializeField] private OrderRecipeTip OrderRecipeTip;

    private readonly Dictionary<Customer, OrderSlotUI> _orderSlotDictionary = new Dictionary<Customer, OrderSlotUI>();

    private void Awake()
    {
        InitializeOrderSlots();
    }

    private void InitializeOrderSlots()
    {
        for (int i = 0; i < OrderSlots.Length; i++)
        {
            if (OrderSlots[i] == null)
            {
                continue;
            }

            OrderSlots[i].Initialize(OrderRecipeTip);
        }

        if (OrderRecipeTip != null)
        {
            OrderRecipeTip.Close();
        }
    }

    private void OnEnable()
    {
        if (GameManager.Inst == null)
        {
            return;
        }

        if (TutorialManager.Inst != null)
        {
            TutorialManager.Inst.OnTutorialStepChanged += RefreshTutorialGuide;
        }

        GameManager.Inst.OnScoreChanged += RefreshScoreText;
        GameManager.Inst.OnTimeChanged += RefreshTimeText;
        GameManager.Inst.OnCustomerOrderAdded += AddOrderSlot;
        GameManager.Inst.OnCustomerWaitRatioChanged += RefreshOrderSlot;
        GameManager.Inst.OnCustomerOrderRemoved += RemoveOrderSlot;

        RefreshDayText();
        RefreshTutorialGuide();
        RefreshTimeText(Mathf.CeilToInt(GameManager.Inst.CurrentTime));
        RefreshScoreText(GameManager.Inst.Score);
    }

    private void OnDisable()
    {
        if (GameManager.Inst == null)
        {
            return;
        }

        if (TutorialManager.Inst != null)
        {
            TutorialManager.Inst.OnTutorialStepChanged -= RefreshTutorialGuide;
        }

        GameManager.Inst.OnScoreChanged -= RefreshScoreText;
        GameManager.Inst.OnTimeChanged -= RefreshTimeText;
        GameManager.Inst.OnCustomerOrderAdded -= AddOrderSlot;
        GameManager.Inst.OnCustomerWaitRatioChanged -= RefreshOrderSlot;
        GameManager.Inst.OnCustomerOrderRemoved -= RemoveOrderSlot;
    }

    private void RefreshTutorialGuide()
    {
        if (TutorialManager.Inst == null)
        {
            RefreshTutorialGuide(TutorialStep.None);
            return;
        }

        RefreshTutorialGuide(
            TutorialManager.Inst.CurrentStep);
    }

    private void RefreshTutorialGuide(TutorialStep tutorialStep)
    {
        if (Text_TutorialGuide == null)
        {
            return;
        }

        bool isTutorial = SaveManager.Inst.SaveData.IsTutorialCompleted == false;

        Text_TutorialGuide.gameObject.SetActive(isTutorial);

        if (isTutorial == false)
        {
            return;
        }

        switch (tutorialStep)
        {
            case TutorialStep.CheckRecipe:
                Text_TutorialGuide.text = "먼저 주문 목록에 커서를 올려 필요한 재료를 확인해보세요! \n(이 창은 클릭 시 사라집니다!)";
                break;

            case TutorialStep.PickIngredient:
                Text_TutorialGuide.text = "[ ← → ↑ ↓ ] 방향키를 눌러 해달이를 움직일 수 있습니다. \n재료 상자에 다가가 [ E ] 키를 눌러 재료를 꺼내보세요!";
                break;

            case TutorialStep.CookFood:
                Text_TutorialGuide.text = "조리대 가까이에서 [ E ] 키를 눌러 재료를 투입할 수 있습니다.\n알맞은 재료를 넣어 요리를 완성해 보세요!";
                break;

            case TutorialStep.ServeFood:
                Text_TutorialGuide.text = "완성된 음식을 손님에게 서빙해보세요!";
                break;

            case TutorialStep.Complete:
                Text_TutorialGuide.text = "첫 서빙 성공! \n 벌써 입소문이 나기 시작했어요!!";
                break;

            default:
                Text_TutorialGuide.text = "손님이 도착";
                break;
        }
    }

    private void RefreshScoreText(int score)
    {
        Text_Score.text = $"Score : {score}";
    }

    private void RefreshTimeText(int currentTime)
    {
        Text_Time.text = $"{currentTime}";
    }

    private void RefreshDayText()
    {
        if (Text_Day == null)
        {
            return;
        }

        Text_Day.text = $"DAY {SaveManager.Inst.SaveData.CurrentDay}";
    }

    private void AddOrderSlot(Customer customer)
    {
        if (customer == null)
        { 
            return; 
        }

        OrderSlotUI emptySlot = GetEmptyOrderSlot();

        if (emptySlot == null)
        {
            return;
        }

        emptySlot.Open(customer.OrderFoodType);
        _orderSlotDictionary.Add(customer, emptySlot);
    }

    private void RefreshOrderSlot(Customer customer, float waitRatio)
    {
        if (_orderSlotDictionary.TryGetValue(customer, out OrderSlotUI slot) == false)
        {
            return;
        }

        slot.RefreshWaitGauge(waitRatio);
    }

    private void RemoveOrderSlot(Customer customer)
    {
        if (_orderSlotDictionary.TryGetValue(customer, out OrderSlotUI slot) == false)
        {
            return;
        }

        slot.Close();
        _orderSlotDictionary.Remove(customer);
    }

    private OrderSlotUI GetEmptyOrderSlot()
    {
        for (int i = 0; i < OrderSlots.Length; i++)
        {
            if(OrderSlots[i] == null)
            {
                continue;
            }

            if (OrderSlots[i].IsUsing == false)
            {
                return OrderSlots[i];
            }
        }    
        return null;
    }
}