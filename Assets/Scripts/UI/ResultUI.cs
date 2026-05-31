using TMPro;
using UnityEngine;

public class ResultUI : UIBase
{
    [SerializeField] private TMP_Text Text_TodayScore;
    [SerializeField] private TMP_Text Text_BestScore;
    [SerializeField] private TMP_Text Text_CompletedOrder;
    [SerializeField] private TMP_Text Text_Satisfaction;

    public void Open()
    {
        gameObject.SetActive(true);

        RefreshTodayScore();
        RefreshResultInfo();
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
}