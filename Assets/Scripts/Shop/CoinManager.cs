using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private int coin = 0;
    public int Coin
    {
        get => coin;
        set
        {
            coin = value;
            OnCoinChanged?.Invoke(coin);
        }
    }
    public delegate void CoinChanged(int newCoinValue);
    public event CoinChanged OnCoinChanged;
    private void Awake()
    {

        Coin = 0;
    }
    public void AddCoin(int amount)
    {
        Coin += amount;
    }
    public void SetCoin(int amount)
    {
        Coin = amount;
        if (Coin < 0)
        {
            Coin = 0;
        }
    }
    public void Getcoin(int amount)
    {
        Coin += amount;
        if (Coin < 0)
        {
            Coin = 0;
        }
    }
    public void RemoveCoin(int amount)
    {
        Coin -= amount;
        if (Coin < 0)
        {
            Coin = 0;
        }
    }

    public void ResetCoin()
    {
        Coin = 0;
    }

}
