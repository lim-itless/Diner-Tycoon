using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddScore(int score)
    {
        Score += score;

        Debug.Log($"현재 점수: {Score}");
    }
}