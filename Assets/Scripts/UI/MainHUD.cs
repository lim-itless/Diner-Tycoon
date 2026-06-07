using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainHUD : UIBase
{
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

        GameManager.Inst.OnScoreChanged += RefreshScoreText;
        GameManager.Inst.OnTimeChanged += RefreshTimeText;
        GameManager.Inst.OnCustomerOrderAdded += AddOrderSlot;
        GameManager.Inst.OnCustomerWaitRatioChanged += RefreshOrderSlot;
        GameManager.Inst.OnCustomerOrderRemoved += RemoveOrderSlot;

        RefreshDayText();
        RefreshTimeText(Mathf.CeilToInt(GameManager.Inst.CurrentTime));
        RefreshScoreText(GameManager.Inst.Score);
    }

    private void OnDisable()
    {
        if (GameManager.Inst == null)
        {
            return;
        }

        GameManager.Inst.OnScoreChanged -= RefreshScoreText;
        GameManager.Inst.OnTimeChanged -= RefreshTimeText;
        GameManager.Inst.OnCustomerOrderAdded -= AddOrderSlot;
        GameManager.Inst.OnCustomerWaitRatioChanged -= RefreshOrderSlot;
        GameManager.Inst.OnCustomerOrderRemoved -= RemoveOrderSlot;
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