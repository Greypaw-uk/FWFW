using UnityEngine;

public class Currency : MonoBehaviour, ICurrency
{
    [SerializeField] private int currentMoney;
    public event System.Action OnMoneyChanged;


    /// <summary>
    /// Gets the current amount of money.
    /// </summary>
    public int GetMoney => currentMoney;
        

    /// <summary>
    /// Adds money to the current amount.
    /// </summary>
    /// <param name="amount"></param>
    public void AddMoney(int amount)
    {
        Debug.Log($"[Currency] Adding money: {amount}");
        
        currentMoney += amount;
        OnMoneyChanged?.Invoke();
    }


    /// <summary>
    /// Removes money from the current amount if sufficient funds exist.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool RemoveMoney(int amount)
    {
        if (currentMoney < amount)
            return false;

        currentMoney -= amount;
        OnMoneyChanged?.Invoke();
        return true;
    }


    /// <summary>
    /// Sets the current amount of money.
    /// </summary>
    /// <param name="amount"></param>
    public void SetMoney(int amount)
    {
        currentMoney = amount;
        OnMoneyChanged?.Invoke();
    }
}
