using Blackjack.Core.Abstractions;
using Blackjack.Core.Game;

namespace Blackjack.Core.Players.Strategies
{
    /*
     BasicBotStrategy
     - A simple, deterministic automated player strategy used for bots and tests.
     - Uses BotStrategySettings to control behavior (when to hit, whether double-down is allowed).
     - Designed to be fast, side-effect free and predictable so it is suitable for unit tests.
     - Typical usage: construct with a settings instance and call Decide(...) during engine play.
     - Gotchas:
       * The engine performs validation (CanDoubleDown/CanSplit) before offering those options;
         this strategy only returns DoubleDown when it is allowed and the hand value is favorable.
       * Because this strategy is deterministic, tests can rely on the same input producing the same output.
    */
    public sealed class BasicBotStrategy : IPlayerStrategy
    {
        private readonly BotStrategySettings _settings;

        /*
         BasicBotStrategy(settings)
         - settings: controls behavioral knobs such as HitUntilValue and AllowDoubleDown.
         - No validation is performed here; callers should ensure a non-null settings object.
        */
        public BasicBotStrategy(BotStrategySettings settings)
        {
            _settings = settings;
        }

        /*
         Decide(context)
         - Returns a PlayerDecision for the provided PlayerDecisionContext.
         - Algorithm:
           1. Compute current hand value.
           2. If double-down is allowed by settings and context and the hand value is 10 or 11,
              prefer DoubleDown (common simple heuristic).
           3. If hand value <= HitUntilValue (from settings), return Hit.
           4. Otherwise return Stand.
         - Characteristics:
           * Deterministic and quick.
           * Respects contextual permissions (context.CanDoubleDown) before choosing double down.
           * Does not attempt advanced strategy (no soft/hard ace branching, no dealer up-card tables).
        */
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
