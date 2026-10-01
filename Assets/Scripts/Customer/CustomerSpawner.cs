using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject CustomerPrefab;
    [SerializeField] private Transform CustomerGroup;
    [SerializeField] private Transform EntranceSpot;
    [SerializeField] private Transform[] WaitSpots;
    [SerializeField] private Transform OrderBubble_Layout;

    [SerializeField] private float _spawnTerm = 1f;

    [SerializeField] private string _tutorialCustomerId = "Customer_Normal_01";
    [SerializeField] private IngredientType _tutorialOrderFoodType = IngredientType.ClamChowder;

    private Customer[] _customers;
    private bool _isSpawning;

    private void Awake()
    {
        _customers = new Customer[WaitSpots.Length];
    }

    public void BeginSpawnCustomers()
    {
        if (_isSpawning == true)
        {
            return;
        }

        StartCoroutine(SpawnAllEmptyWaitSpotsCoroutine());
    }

    private IEnumerator SpawnAllEmptyWaitSpotsCoroutine()
    {
        _isSpawning = true;

        for (int i = 0; i < WaitSpots.Length; i++)
        {
            if (_customers[i] != null)
            {
                continue;
            }

            SpawnCustomer(i);

            yield return new WaitForSeconds(_spawnTerm);
        }

        _isSpawning = false;
    }

    public void SpawnTutorialCustomer()
    {
        ClearCustomers();

        if (WaitSpots == null || WaitSpots.Length == 0)
        {
            return;
        }

        CustomerData customerData = GameDataManager.Inst.GetCustomerData(_tutorialCustomerId);

        if (customerData == null)
        {
            return;
        }

        Customer customer = CreateCustomer();

        if (customer == null)
        {
            return;
        }

        customer.InitializeData(customerData);
        customer.SetOrderFoodType(_tutorialOrderFoodType);

        _customers[0] = customer;

        customer.Initialize(OrderBubble_Layout, WaitSpots[0].position, EntranceSpot.position, OnCustomerExitCompleted);

        TutorialManager.Inst.SetCustomerTarget(customer.CustomerAnchorTransform);

        GameManager.Inst.AddVisitCustomer();
    }

    private Customer CreateCustomer()
    {
        int instanceId = GameObjectManager.Inst.CreateObject(CustomerPrefab, EntranceSpot.position, Quaternion.identity);

        GameObject customerObject = GameObjectManager.Inst.GetObject(instanceId);

        if (customerObject == null)
        {
            return null;
        }

        customerObject.transform.SetParent(CustomerGroup);

        Customer customer = customerObject.GetComponent<Customer>();

        if (customer == null)
        {
            GameObjectManager.Inst.RemoveObject(customerObject);
            return null;
        }

        return customer;
    }

    private void SpawnCustomer(int waitIndex)
    {
        if (IsValidWaitIndex(waitIndex) == false)
        {
            return;
        }

        CustomerData customerData = GameDataManager.Inst.GetRandomCustomerData();

        if (customerData == null)
        {
            return;
        }

        Customer customer = CreateCustomer();

        if (customer == null)
        {
            return;
        }

        customer.InitializeData(customerData);

        _customers[waitIndex] = customer;

        customer.Initialize(OrderBubble_Layout, WaitSpots[waitIndex].position, EntranceSpot.position, OnCustomerExitCompleted);

        GameManager.Inst.AddVisitCustomer();
    }

    private void OnCustomerExitCompleted(Customer customer)
    {
        int emptyIndex = GetCustomerIndex(customer);

        if (emptyIndex < 0)
        {
            return;
        }

        _customers[emptyIndex] = null;

        StartCoroutine(SpawnEmptySpotAfterDelayCoroutine(emptyIndex));
    }

    private IEnumerator SpawnEmptySpotAfterDelayCoroutine(int waitIndex)
    {
        yield return new WaitForSeconds(_spawnTerm);

        if (IsValidWaitIndex(waitIndex) == false)
        {
            yield break;
        }

        if (_customers[waitIndex] != null)
        {
            yield break;
        }

        SpawnCustomer(waitIndex);
    }

    private bool IsValidWaitIndex(int waitIndex)
    {
        if (waitIndex < 0 || waitIndex >= WaitSpots.Length)
        {
            return false;
        }

        if (WaitSpots[waitIndex] == null)
        {
            return false;
        }

        return true;
    }

    private int GetCustomerIndex(Customer customer)
    {
        for (int i = 0; i < _customers.Length; i++)
        {
            if (_customers[i] == customer)
            {
                return i;
            }
        }

        return -1;
    }

    public void ClearCustomers()
    {
        StopAllCoroutines();

        _isSpawning = false;

        for (int i = 0; i < _customers.Length; i++)
        {
            if (_customers[i] == null)
            {
                continue;
            }

            _customers[i].ClearRuntime();

            GameObjectManager.Inst.RemoveObject(_customers[i].gameObject);

            _customers[i] = null;
        }
    }
}