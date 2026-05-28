using System;
using System.Collections.Generic;

[Serializable]
public class CustomerSpawnData
{
    public string CustomerId;
    public int Weight;
}

[Serializable]
public class CustomerSpawnDataList
{
    public List<CustomerSpawnData> CustomerSpawnDatas;
}