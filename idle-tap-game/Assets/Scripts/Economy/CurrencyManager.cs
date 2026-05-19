using System;

namespace IdleTapGame.Economy
{
    /// <summary>
    /// Single soft currency ("Gold"). Raises <see cref="GoldChanged"/> on mutation.
    /// </summary>
    public class CurrencyManager
    {
        public double Gold { get; private set; }

        public event Action<double> GoldChanged;

        public void SetGold(double amount)
        {
            Gold = amount < 0d ? 0d : amount;
            GoldChanged?.Invoke(Gold);
        }

        public void Add(double amount)
        {
            if (amount <= 0d) return;
            Gold += amount;
            GoldChanged?.Invoke(Gold);
        }

        public bool TrySpend(double amount)
        {
            if (amount < 0d || Gold < amount) return false;
            Gold -= amount;
            GoldChanged?.Invoke(Gold);
            return true;
        }
    }
}
