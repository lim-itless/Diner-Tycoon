using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    public int Score { get; private set; }

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
    }

    public void AddScore(int score)
    {
        Score += score;

        Debug.Log($"현재 점수: {Score}");
    }
}