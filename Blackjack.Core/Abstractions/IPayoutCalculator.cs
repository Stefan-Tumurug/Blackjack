using Blackjack.Core.Game;

namespace Blackjack.Core.Abstractions
{
    // Abstraction to encapsulate payout and settlement rules outside of the GameEngine.
    /*
     IPayoutCalculator
     - Keeps payout logic separate from game flow so the GameEngine does not become a "god class".
     - Allows swapping different payout implementations for testing or rule variants (e.g. different blackjack payouts).
     - Implementations compute the net change to a player's bankroll for a single hand given the base bet,
       the resolved round result and whether the hand was doubled down.
     - Return value semantics:
         * Positive => player gains this amount (credit to bankroll).
         * Negative => player loses this amount (debit from bankroll).
         * Zero => no net change (push).
    */
    public interface IPayoutCalculator
    {
        /*
         CalculateNetChange
         - baseBet: the original bet amount placed for the hand (positive integer).
         - result: the resolved RoundResult for the hand (PlayerWin, DealerWin, Push, etc.).
         - doubledDown: true when the player doubled down (typically indicates the stake was doubled).
         - Returns the net change in the player's bankroll (positive, negative or zero).
         - The method should incorporate doubledDown into its calculation so callers can apply the
           returned value directly to the player's balance.
        */
        int CalculateNetChange(int baseBet, RoundResult result, bool doubledDown);
    }
}
