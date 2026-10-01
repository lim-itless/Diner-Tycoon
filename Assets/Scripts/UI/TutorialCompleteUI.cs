using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialCompleteUI : UIBase
{
    [SerializeField] private TMP_Text Text_Message;
    [SerializeField] private float _showDuration = 1f;

    public override void Open()
    {
        base.Open();

        if (Text_Message != null)
        {
            Text_Message.text = "Tutorial Complete!";
        }

        StartCoroutine(CloseAfterDelayCoroutine());
    }

    private IEnumerator CloseAfterDelayCoroutine()
    {
        yield return new WaitForSeconds(_showDuration);

        UIManager.Inst.CloseUI<TutorialCompleteUI>();

        GameManager.Inst.StartGame();
    }
}