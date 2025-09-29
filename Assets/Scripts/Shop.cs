using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private MoneyManager _moneyManager;
    [SerializeField] private BuildingPlacementManager _buildingPlacementManager;

    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _testCost = 35;
    public void Buy()
    {
        if (_moneyManager.SpendMoney(_testCost))
        {
            _buildingPlacementManager.StartPlacing(_prefab);
        }
    }

    public void Refund()
    {
        _moneyManager.AddMoney(_testCost);
    }
}
