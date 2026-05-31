using System;

[Serializable]
public class GameResultModel
{
    public int BestScore;

    public int CompletedOrderCount;

    public int VisitCustomerCount;

    public int FailedCustomerCount;

    public int SatisfactionPercent;
}