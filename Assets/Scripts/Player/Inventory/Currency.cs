using UnityEngine;

public class Currency : MonoBehaviour, ICurrency
{
    [SerializeField] private int currentMoney;
    public event System.Action OnMoneyChanged;


    public int GetMoney => currentMoney;
        
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke();
    }

    public bool RemoveMoney(int amount)
    {
        if (currentMoney < amount)
            return false;

        currentMoney -= amount;
        OnMoneyChanged?.Invoke();
        return true;
    }

    public void SetMoney(int amount)
    {
        currentMoney = amount;
        OnMoneyChanged?.Invoke();
    }
}
