using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Inst { get; private set; }

    [SerializeField] private List<UIBase> _uiList;

    private readonly Dictionary<System.Type, UIBase> _uiDictionary = new Dictionary<System.Type, UIBase>();

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;

        Initialize();
    }

    private void Initialize()
    {
        _uiDictionary.Clear();

        for (int i = 0; i < _uiList.Count; i++)
        {
            UIBase ui = _uiList[i];

            if (ui == null)
            {
                continue;
            }

            System.Type uiType = ui.GetType();

            if (_uiDictionary.ContainsKey(uiType) == true)
            {
                Debug.LogWarning($"중복 UI 등록 : {uiType.Name}");
                continue;
            }

            _uiDictionary.Add(uiType, ui);
        }
    }

    public T GetUI<T>() where T : UIBase
    {
        System.Type uiType = typeof(T);

        if (_uiDictionary.ContainsKey(uiType) == false)
        {
            return null;
        }

        return _uiDictionary[uiType] as T;
    }

    public void OpenUI<T>() where T : UIBase
    {
        T ui = GetUI<T>();

        if (ui == null)
        {
            return;
        }

        ui.Open();
    }

    public void CloseUI<T>() where T : UIBase
    {
        T ui = GetUI<T>();

        if (ui == null)
        {
            return;
        }

        ui.Close();
    }
}