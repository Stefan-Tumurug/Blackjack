using System;

namespace Blackjack.Core.Betting
{
    // Bet
    // - Value object that represents a single bet amount.
    // - Encapsulates validation so callers don't need to repeat checks before constructing a bet.
    // - Invariant: Amount is always a positive integer (> 0).
    // - Keeps validation close to the data which makes higher-level code simpler and safer.
    public sealed class Bet
    {
        // The placed bet amount. Guaranteed to be > 0 for any constructed instance.
        public int Amount { get; }

        /*
         Bet(amount)
         - Creates a new Bet value object.
         - Validates that `amount` is greater than zero and throws ArgumentOutOfRangeException otherwise.
         - By throwing on invalid input the type guarantees its invariant and allows callers to
           assume a valid bet once an instance exists.
        */
        public Bet(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Bet amount must be greater than zero.");
            }
            Amount = amount;
        }
    }
}
