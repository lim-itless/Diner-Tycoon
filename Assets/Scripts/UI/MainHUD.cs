using TMPro;
using UnityEngine;

public class MainHUD : UIBase
{
    [SerializeField] private TMP_Text Text_Score;
    [SerializeField] private TMP_Text Text_Time;

    private void OnEnable()
    {
        if (GameManager.Inst == null)
        {
            return;
        }

        GameManager.Inst.OnScoreChanged += RefreshScoreText;
        GameManager.Inst.OnTimeChanged += RefreshTimeText;

        RefreshScoreText(GameManager.Inst.Score);
        RefreshTimeText(Mathf.CeilToInt(GameManager.Inst.CurrentTime));
    }

    private void OnDisable()
    {
        if (GameManager.Inst == null)
        {
            return;
        }

        GameManager.Inst.OnScoreChanged -= RefreshScoreText;
        GameManager.Inst.OnTimeChanged -= RefreshTimeText;
    }

    private void RefreshScoreText(int score)
    {
        Text_Score.text = $"Score : {score}";
    }

    private void RefreshTimeText(int currentTime)
    {
        Text_Time.text = $"{currentTime}";
    }
}