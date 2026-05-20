using TMPro;
using UnityEngine;

public class ResultUI : UIBase
{
    [SerializeField] private TMP_Text Text_TodayScore;

    public void Open()
    {
        gameObject.SetActive(true);

        RefreshTodayScore();
    }

    private void RefreshTodayScore()
    {
        Text_TodayScore.text =
            $"Today Score : {GameManager.Inst.Score}";
    }
}