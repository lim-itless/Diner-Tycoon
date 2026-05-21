using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _gameTime = 60f;
    [SerializeField] private ResultUI ResultUI;

    public static GameManager Inst { get; private set; }

    public int Score { get; private set; }
    public float CurrentTime { get; private set; }
    public bool IsGameEnd { get; private set; }

    private void Awake()
    {
        if (Inst != null)
        {
            Destroy(gameObject);
            return;
        }

        Inst = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        HandleGameTimer();
    }

    public void StartGame()
    {
        CurrentTime = _gameTime;
        IsGameEnd = false;

        Debug.Log("게임 시작!!");
    }

    private void HandleGameTimer()
    {
        if (IsGameEnd == true)
        {
            return;
        }

        CurrentTime -= Time.deltaTime;

        if (CurrentTime > 0)
        {
            return;
        }

        CurrentTime = 0;
        EndGame();
    }

    public void AddScore(int score)
    {
        Score += score;

        Debug.Log($"현재 점수 : {Score}");
    }

    private void EndGame()
    {
        IsGameEnd = true;

        ResultUI.Open();

        Debug.Log($"게임 종료!! 최종 점수 : {Score}");
    }
}