using System;

namespace Blackjack.Core.Betting
{
    // Holds a player's bankroll (total money available for betting) and provides methods to place bets and update the bankroll based on wins/losses.
    // GameEngine should not be responsible for validating or protecting balance
    public sealed class Bankroll
    {
        public int Balance { get; private set; }

        public Bankroll(int startingBalance)
        {
            if (startingBalance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingBalance), "Starting balance cannot be negative.");
            }

            Balance = startingBalance;
        }

        public bool CanPlaceBet(int amount)
        {
            return amount > 0 && amount <= Balance;
        }

        public void ApplyNetChange(int netChange)
        {
            int newBalance = Balance + netChange;
            if (newBalance < 0)
            {
                throw new InvalidOperationException("Net change cannot result in a negative balance.");
            }
            Balance = newBalance;
        }

        public void Reset(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            Balance = amount;
        }

    }
}
