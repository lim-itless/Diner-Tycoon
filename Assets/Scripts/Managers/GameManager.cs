using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _gameTime = 60f;
    [SerializeField] private ResultUI ResultUI;
    [SerializeField] private CustomerSpawner CustomerSpawner;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnTimeChanged;

    private int _lastTime;

    public static GameManager Inst { get; private set; }

    public int Score { get; private set; }
    public float CurrentTime { get; private set; }
    public bool IsGamePlaying { get; private set; }
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

    //private void Start()
    //{
    //    StartGame();
    //}

    private void Update()
    {
        HandleGameTimer();
    }

    public void StartGame()
    {
        Score = 0;
        CurrentTime = _gameTime;
        IsGameEnd = false;
        IsGamePlaying = true;

        _lastTime = Mathf.CeilToInt(CurrentTime);

        OnScoreChanged?.Invoke(Score);
        OnTimeChanged?.Invoke(_lastTime);

        SoundManager.Inst.PlayMainBGM();

        CustomerSpawner.BeginSpawnCustomers();
    }

    private void HandleGameTimer()
    {
        if (IsGamePlaying == false)
        {
            return;
        }

        if (IsGameEnd == true)
        {
            return;
        }

        CurrentTime -= Time.deltaTime;

        int currentTime = Mathf.CeilToInt(CurrentTime);

        if (_lastTime != currentTime)
        {
            _lastTime = currentTime;
            OnTimeChanged?.Invoke(_lastTime);
        }

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

        OnScoreChanged?.Invoke(Score);

        Debug.Log($"현재 점수 : {Score}");
    }

    private void EndGame()
    {
        IsGameEnd = true;
        IsGamePlaying = false;

        ResultUI.Open();

        Debug.Log($"게임 종료!! 최종 점수 : {Score}");
    }
}