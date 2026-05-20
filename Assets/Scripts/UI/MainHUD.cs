using TMPro;
using UnityEngine;

public class MainHUD : UIBase
{
    [SerializeField] private TMP_Text Text_Score;
    [SerializeField] private TMP_Text Text_Time;

    private void Update()
    {
        RefreshScoreText();
        RefreshTimeText();
    }

    private void RefreshScoreText()
    {
        Text_Score.text = $"Score : {GameManager.Inst.Score}";
    }
    private void RefreshTimeText()
    {
        int currentTime = Mathf.CeilToInt(GameManager.Inst.CurrentTime);

        Text_Time.text = $"{currentTime}";
    }
}
