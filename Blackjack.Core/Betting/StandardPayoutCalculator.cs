using Blackjack.Core.Abstractions;
using Blackjack.Core.Game;

namespace Blackjack.Core.Betting
{
    // StandardPayoutCalculator
    // - Encapsulates the default blackjack payout rules used by the game.
    // - Keeps payout concerns separate from GameEngine by implementing IPayoutCalculator.
    // - Design notes:
    //   * Double down is modeled by doubling the effective stake before settlement.
    //   * Positive return value indicates a credit to the player's bankroll.
    //   * Negative return value indicates a debit from the player's bankroll.
    //   * Push (tie) yields no net change.
    //   * This class performs no input validation (e.g. for non-positive baseBet) — callers are expected
    //     to ensure the inputs are valid.
    public sealed class StandardPayoutCalculator : IPayoutCalculator
    {
        /* 
         CalculateNetChange
         - baseBet: the original bet amount placed for the hand (assumed > 0).
         - result: the resolved RoundResult for the hand.
         - doubledDown: when true, the effective stake is doubled before computing net change.
         - Returns the net change to apply to the player's bankroll:
             * PlayerWin  => +stake
             * DealerWin  => -stake
             * Push       => 0
         - Examples:
             * baseBet = 10, doubledDown = false, PlayerWin  => +10
             * baseBet = 10, doubledDown = true,  PlayerWin  => +20
             * baseBet = 5,  doubledDown = true,  DealerWin  => -10
        */
        public int CalculateNetChange(int baseBet, RoundResult result, bool doubledDown)
        {
            int bet = doubledDown ? baseBet * 2 : baseBet;

            return result switch
            {
                RoundResult.PlayerWin => bet,
                RoundResult.DealerWin => -bet,
                RoundResult.Push => 0,
                _ => 0
            };
        }
    }
}
