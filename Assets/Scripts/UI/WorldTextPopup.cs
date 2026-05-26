using TMPro;
using UnityEngine;

public class WorldTextPopup : UIBase
{
    [SerializeField] private TMP_Text Text_content;

    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        transform.position += Vector3.up * _moveSpeed * Time.deltaTime;
    }

    public void SetText(string text)
    {
        Text_content.text = text;
    }

}
