using TMPro;
using UnityEngine;

public class MainHUD : UIBase
{
    [SerializeField] private TMP_Text Text_Score;

    private void Update()
    {
        RefreshScoreText();
    }

    private void RefreshScoreText()
    {
        Text_Score.text = $"Score : {GameManager.Inst.Score}";
    }
}
