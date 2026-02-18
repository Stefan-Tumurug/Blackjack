using Blackjack.Core.Game;

namespace Blackjack.Core.Abstractions
{
    // Keeps payout rules separate from GameEngine.
    // This prevents GameEngine from becoming a "god class" as features grow.
    public interface IPayoutCalculator
    {
        int CalculateNetChange(int baseBet, RoundResult result, bool doubledDown);
    }
}
