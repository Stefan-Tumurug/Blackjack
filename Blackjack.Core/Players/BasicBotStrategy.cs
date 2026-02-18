using Blackjack.Core.Abstractions;
using Blackjack.Core.Game;

namespace Blackjack.Core.Players
{
    // Simple deterministic bot:
    // Hit on 16 or lower, stand on 17 or higher.
    // Keeping it deterministic makes it easy to test.
    public sealed class BasicBotStrategy : IPlayerStrategy
    {
        public PlayerDecision Decide(PlayerDecisionContext context)
        {
            int value = context.PlayerHand.GetValue();

            if (value <= 16)
            {
                return PlayerDecision.Hit;
            }

            return PlayerDecision.Stand;
        }
    }
}
