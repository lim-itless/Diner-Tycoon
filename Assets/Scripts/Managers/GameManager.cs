using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _gameTime = 60f;
    [SerializeField] private CustomerSpawner CustomerSpawner;
    [SerializeField] private CookStation CookStation;
    [SerializeField] private PlayerController PlayerController;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnTimeChanged;
    public event Action<Customer> OnCustomerOrderAdded;
    public event Action<Customer, float> OnCustomerWaitRatioChanged;
    public event Action<Customer> OnCustomerOrderRemoved;

    private int _lastTime;

    public static GameManager Inst { get; private set; }

    public GameResultModel GameResultModel { get; private set; }
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

    private void Update()
    {
        HandleGameTimer();
    }

    public void StartGame()
    {
        ClearDayRuntime();
        ResetGameResult();

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

        ClearDayRuntime();

        RefreshSatisfaction();
        RefreshBestScore();
        RefreshDay();

        SaveManager.Inst.SaveGame();

        SoundManager.Inst.StopBGM();

        UIManager.Inst.OpenUI<ResultUI>();
    }

    public void AddVisitCustomer()
    {
        GameResultModel.VisitCustomerCount++;
    }

    public void AddCompletedOrder()
    {
        GameResultModel.CompletedOrderCount++;
    }

    public void AddFailedCustomer()
    {
        GameResultModel.FailedCustomerCount++;
    }

    private void RefreshDay()
    {
        SaveManager.Inst.SaveData.CurrentDay++;
    }

    private void RefreshSatisfaction()
    {
        if (GameResultModel.VisitCustomerCount <= 0)
        {
            GameResultModel.SatisfactionPercent = 100;
            return;
        }

        float ratio = (float)GameResultModel.CompletedOrderCount / GameResultModel.VisitCustomerCount;

        GameResultModel.SatisfactionPercent = Mathf.RoundToInt(ratio * 100f);
    }

    private void RefreshBestScore()
    {
        GameResultModel.BestScore = SaveManager.Inst.SaveData.BestScore;

        if (Score <= GameResultModel.BestScore)
        {
            return;
        }

        GameResultModel.BestScore = Score;
        SaveManager.Inst.SaveData.BestScore = Score;
    }

    private void ResetGameResult()
    {
        GameResultModel = new GameResultModel();

        GameResultModel.BestScore = SaveManager.Inst.SaveData.BestScore;
    }

    private void ClearDayRuntime()
    {
        CustomerSpawner.ClearCustomers();

        if (CookStation != null)
        {
            CookStation.ResetStation();
        }

        if (PlayerController != null)
        {
            PlayerController.ResetPlayerState();
        }
    }

    public void AddCustomerOrder(Customer customer)
    {
        OnCustomerOrderAdded?.Invoke(customer);
    }

    public void RefreshCustomerWaitRatio(Customer customer, float waitRatio)
    {
        OnCustomerWaitRatioChanged?.Invoke(customer, waitRatio);
    }

    public void RemoveCustomerOrder(Customer customer)
    {
        OnCustomerOrderRemoved?.Invoke(customer);
    }
}