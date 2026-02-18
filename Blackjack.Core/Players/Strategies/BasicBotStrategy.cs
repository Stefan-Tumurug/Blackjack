using Blackjack.Core.Abstractions;
using Blackjack.Core.Game;

namespace Blackjack.Core.Players.Strategies
{
    // Deterministic bot strategy.
    // Uses settings to decide when to hit/stand and whether to double down.
    public sealed class BasicBotStrategy : IPlayerStrategy
    {
        private readonly BotStrategySettings _settings;

        public BasicBotStrategy(BotStrategySettings settings)
        {
            _settings = settings;
        }

        public PlayerDecision Decide(PlayerDecisionContext context)
        {
            int value = context.PlayerHand.Hand.GetValue();

            // Optional: simple double down rule.
            // Double down only if allowed, possible, and hand value is "good for doubling".
            if (_settings.AllowDoubleDown && context.CanDoubleDown)
            {
                if (value == 10 || value == 11)
                {
                    return PlayerDecision.DoubleDown;
                }
            }

            if (value <= _settings.HitUntilValue)
            {
                return PlayerDecision.Hit;
            }

            return PlayerDecision.Stand;
        }
    }
}
