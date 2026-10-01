using System.Collections;
using TMPro;
using UnityEngine;

public class DayStartUI : UIBase
{
    [SerializeField] private TMP_Text Text_Day;
    [SerializeField] private float _showDuration = 1f;

    public override void Open()
    {
        base.Open();

        RefreshDayText();

        StartCoroutine(CloseAfterDelayCoroutine());
    }

    private void RefreshDayText()
    {
        if (Text_Day == null)
        {
            return;
        }

        Text_Day.text = $"DAY {SaveManager.Inst.SaveData.CurrentDay}";
    }

    private IEnumerator CloseAfterDelayCoroutine()
    {
        yield return new WaitForSeconds(_showDuration);

        UIManager.Inst.CloseUI<DayStartUI>();

        GameManager.Inst.BeginGamePlay();
    }
}