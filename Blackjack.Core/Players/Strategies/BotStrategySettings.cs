using System;

namespace Blackjack.Core.Players.Strategies
{
    // Configuration object for bot behavior.
    // Keeps BasicBotStrategy open for extension without adding more if/else logic.
    public sealed class BotStrategySettings
    {
        public int HitUntilValue { get; }
        public bool AllowDoubleDown { get; }

        public BotStrategySettings(int hitUntilValue, bool allowDoubleDown)
        {
            if (hitUntilValue < 0 || hitUntilValue > 21)
            {
                throw new ArgumentOutOfRangeException(nameof(hitUntilValue));
            }

            HitUntilValue = hitUntilValue;
            AllowDoubleDown = allowDoubleDown;
        }

        public static BotStrategySettings Conservative()
        {
            // Stands earlier, less aggressive.
            return new BotStrategySettings(hitUntilValue: 15, allowDoubleDown: false);
        }

        public static BotStrategySettings Standard()
        {
            return new BotStrategySettings(hitUntilValue: 16, allowDoubleDown: true);
        }

        public static BotStrategySettings Aggressive()
        {
            // Hits longer, higher risk.
            return new BotStrategySettings(hitUntilValue: 17, allowDoubleDown: true);
        }
    }
}
