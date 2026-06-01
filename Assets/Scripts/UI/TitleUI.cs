using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UIBase
{
    [SerializeField] private Button Button_Start;
    [SerializeField] private Button Button_Quit;

    private void Awake()
    {
        BindButtonEvents();
    }

    private void BindButtonEvents()
    {
        BindOnClickButtonEvent(Button_Start, OnClickStartButton);
        BindOnClickButtonEvent(Button_Quit, OnClickQuitButton);
    }

    private void OnClickStartButton()
    {
        SoundManager.Inst.PlayButtonClickSFX();

        UIManager.Inst.CloseUI<TitleUI>();
        UIManager.Inst.OpenUI<MainHUD>();

        GameManager.Inst.StartGame();
    }

    private void OnClickQuitButton()
    {
        SoundManager.Inst.PlayButtonClickSFX();
        Application.Quit();
    }
}