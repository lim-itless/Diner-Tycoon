using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    protected void BindOnClickButtonEvent(Button button, UnityAction action)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }
}