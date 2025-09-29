using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [Header("Money UI Settings")]
    [SerializeField] private TMP_Text _moneyTextUI;

    [SerializeField] private int _amount;
    public int Amount => _amount;

    private void Start()
    {
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        _amount += amount;
        UpdateUI();
    }


    public bool SpendMoney(int amount)
    {
        if (amount <= _amount)
        {
            _amount -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI()
    {
        _moneyTextUI.text = _amount.ToString();
    }
}
