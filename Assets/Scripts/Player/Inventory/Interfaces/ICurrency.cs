public interface ICurrency
{
    int GetMoney { get; }
    event System.Action OnMoneyChanged;

    void AddMoney(int amount);
    bool RemoveMoney(int amount);
    void SetMoney(int amount);
}