using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UIBase
{
    [SerializeField] private Button Button_Start;
    [SerializeField] private Button Button_Quit;
    [SerializeField] private GameObject Object_MainHUD;

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
        gameObject.SetActive(false);
        Object_MainHUD.SetActive(true);

        SoundManager.Inst.PlayButtonClickSFX();
        GameManager.Inst.StartGame();
    }

    private void OnClickQuitButton()
    {
        SoundManager.Inst.PlayButtonClickSFX();
        Application.Quit();
    }
}