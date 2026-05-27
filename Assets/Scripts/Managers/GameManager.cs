using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _gameTime = 60f;
    [SerializeField] private ResultUI ResultUI;
    [SerializeField] private CustomerSpawner CustomerSpawner;

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
        SoundManager.Inst.PlayMainBGM();

        CurrentTime = _gameTime;
        IsGameEnd = false;
        IsGamePlaying = true;

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
        IsGamePlaying = false;

        ResultUI.Open();

        Debug.Log($"게임 종료!! 최종 점수 : {Score}");
    }
}