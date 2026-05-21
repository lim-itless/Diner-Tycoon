using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject CustomerPrefab;
    [SerializeField] private Transform CustomerGroup;
    [SerializeField] private Transform EntranceSpot;
    [SerializeField] private Transform[] QueueSpots;
    [SerializeField] private Transform OrderBubble_Layout;

    private void Start()
    {
        SpawnCustomers();
    }

    private void SpawnCustomers()
    {
        for (int i = 0; i < QueueSpots.Length; i++)
        {
            SpawnCustomer(QueueSpots[i]);
        }
    }

    private void SpawnCustomer(Transform queueSpot)
    {
        if (queueSpot == null)
        {
            return;
        }

        int instanceId = GameObjectManager.Inst.CreateObject(CustomerPrefab, EntranceSpot.position, Quaternion.identity);

        GameObject customerObject = GameObjectManager.Inst.GetObject(instanceId);

        if (customerObject == null)
        {
            return;
        }
        
        customerObject.transform.SetParent(CustomerGroup);

        Customer customer = customerObject.GetComponent<Customer>();

        if(customer == null)
        {
            return; 
        }

        customer.Initialize(OrderBubble_Layout);
        customer.MoveToQueueSpot(queueSpot.position);
    }
}