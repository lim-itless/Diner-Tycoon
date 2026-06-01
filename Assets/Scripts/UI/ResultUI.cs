using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : UIBase
{
    [SerializeField] private TMP_Text Text_TodayScore;
    [SerializeField] private TMP_Text Text_BestScore;
    [SerializeField] private TMP_Text Text_CompletedOrder;
    [SerializeField] private TMP_Text Text_Satisfaction;

    [SerializeField] private Button Button_NextDay;
    [SerializeField] private Button Button_Title;

    private void Awake()
    {
        BindButtonEvents();
    }

    public override void Open()
    {
        base.Open();
        Refresh();
    }

    public void Refresh()
    {
        RefreshTodayScore();
        RefreshResultInfo();
    }

    private void BindButtonEvents()
    {
        BindOnClickButtonEvent(Button_NextDay, OnClickNextDay);
        BindOnClickButtonEvent(Button_Title, OnClickTitle);
    }

    private void RefreshTodayScore()
    {
        Text_TodayScore.text = $"{GameManager.Inst.Score}";
    }

    private void RefreshResultInfo()
    {
        GameResultModel resultModel = GameManager.Inst.GameResultModel;

        Text_BestScore.text = resultModel.BestScore.ToString();
        Text_CompletedOrder.text = resultModel.CompletedOrderCount.ToString();
        Text_Satisfaction.text = $"{resultModel.SatisfactionPercent}%";
    }

    public void OnClickNextDay()
    {
        SoundManager.Inst.PlayButtonClickSFX();
        UIManager.Inst.CloseUI<ResultUI>();
        UIManager.Inst.OpenUI<MainHUD>();
        GameManager.Inst.StartGame();
    }

    public void OnClickTitle()
    {
        SoundManager.Inst.PlayButtonClickSFX();
        UIManager.Inst.CloseUI<ResultUI>();
        UIManager.Inst.CloseUI<MainHUD>();
        UIManager.Inst.OpenUI<TitleUI>();
    }
}