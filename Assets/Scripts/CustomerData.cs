using System;
using System.Collections.Generic;

[Serializable]
public class CustomerData
{
    public string Id;
    public string Name;
    public string CustomerType;
    public float WaitTime;
    public float MoveSpeed;
    public int RewardScore;

    public CustomerType GetCustomerType()
    {
        if (Enum.TryParse(CustomerType, out CustomerType customerType) == false)
        {
            return global::CustomerType.Normal;
        }

        return customerType;
    }
}

[Serializable]
public class CustomerDataList
{
    public List<CustomerData> CustomerDatas;
}