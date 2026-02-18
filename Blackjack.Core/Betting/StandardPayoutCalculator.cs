using Blackjack.Core.Abstractions;
using Blackjack.Core.Game;

namespace Blackjack.Core.Betting
{
    // Standard payout rules:
    // PlayerWin: +bet
    // DealerWin: -bet
    // Push: 0
    // Double down is represented as a 2x multiplier on the base bet.
    public sealed class StandardPayoutCalculator : IPayoutCalculator
    {
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
