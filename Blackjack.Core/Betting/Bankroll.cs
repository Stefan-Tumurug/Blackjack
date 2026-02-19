using System;

namespace Blackjack.Core.Betting
{
    // Bankroll
    // - Encapsulates a player's available money used for betting.
    // - Responsible for simple validation and application of net changes (wins/losses).
    // - Keeps responsibility for balance management out of the GameEngine so the engine
    //   does not need to reason about negative balances or bet validation.
    // - Invariants:
    //     * Balance is always >= 0.
    //     * Starting balance provided to the constructor or Reset must be >= 0.
    public sealed class Bankroll
    {
        // Current available balance. Private setter enforces controlled updates through methods.
        public int Balance { get; private set; }

        /*
         Bankroll(startingBalance)
         - Creates a new Bankroll initialized with `startingBalance`.
         - Throws ArgumentOutOfRangeException when startingBalance is negative.
         - After construction the Balance property equals startingBalance.
        */
        public Bankroll(int startingBalance)
        {
            if (startingBalance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingBalance), "Starting balance cannot be negative.");
            }

            Balance = startingBalance;
        }

        /*
         CanPlaceBet(amount)
         - Returns true when `amount` is a positive integer and does not exceed the current Balance.
         - This helper is convenient for UI or strategy code that needs a quick pre-check
           before attempting to place a bet.
        */
        public bool CanPlaceBet(int amount)
        {
            return amount > 0 && amount <= Balance;
        }

        /*
         ApplyNetChange(netChange)
         - Applies the signed net change to the current Balance.
           * Positive netChange => credit (player won).
           * Negative netChange => debit (player lost).
         - Throws InvalidOperationException if applying the change would result in a negative Balance.
         - This method centralizes the rule that Balance must remain non-negative.
         - Example: when a player loses their bet of 10, caller passes -10; when a player wins 15, caller passes +15.
        */
        public void ApplyNetChange(int netChange)
        {
            int newBalance = Balance + netChange;
            if (newBalance < 0)
            {
                throw new InvalidOperationException("Net change cannot result in a negative balance.");
            }
            Balance = newBalance;
        }

        /*
         Reset(amount)
         - Replaces the current Balance with `amount`.
         - Throws ArgumentOutOfRangeException if `amount` is negative.
         - Useful when starting a new session or test run to set a known bankroll value.
        */
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
