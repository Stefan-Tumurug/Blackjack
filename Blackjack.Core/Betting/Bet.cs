using System;

namespace Blackjack.Core.Betting
{
    // Value object for a single bet amount.
    // Validation lives here to keep callers simple and safe
    public sealed class Bet
    {
        public int Amount { get; }

        public Bet(int amount)
        {
            if (amount <=0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");
            }
            Amount = amount;
        }
    }
}
