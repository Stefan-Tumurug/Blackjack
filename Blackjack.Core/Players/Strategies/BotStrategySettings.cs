using System;

namespace Blackjack.Core.Players.Strategies
{
    // Configuration object for bot behavior.
    // Keeps BasicBotStrategy open for extension without adding more if/else logic.
    /*
     BotStrategySettings
     - Encapsulates tunable parameters that control how a deterministic bot behaves.
     - Separates configuration from algorithm so strategies remain simple and testable.
     - Properties:
         * HitUntilValue  - The inclusive threshold. The bot will Hit when its hand value <= this value.
         * AllowDoubleDown - When true, the bot may choose to double down (subject to engine/context checks).
     - Invariants:
         * HitUntilValue is in the range [0, 21].
     - Usage:
         * Create via the constructor for custom settings or use the provided presets:
           - Conservative(), Standard(), Aggressive()
    */
    public sealed class BotStrategySettings
    {
        public int HitUntilValue { get; }
        public bool AllowDoubleDown { get; }

        /*
         BotStrategySettings(hitUntilValue, allowDoubleDown)
         - Validates and stores settings.
         - Throws ArgumentOutOfRangeException when hitUntilValue is outside 0..21.
         - Keep constructor simple so tests can construct specific behaviors easily.
        */
        public BotStrategySettings(int hitUntilValue, bool allowDoubleDown)
        {
            if (hitUntilValue < 0 || hitUntilValue > 21)
            {
                throw new ArgumentOutOfRangeException(nameof(hitUntilValue));
            }

            HitUntilValue = hitUntilValue;
            AllowDoubleDown = allowDoubleDown;
        }

        /*
         Conservative()
         - Preset settings for a conservative bot:
           * Hits only up to 15 (stands earlier).
           * Does not double down.
         - Use in tests or demos when you want lower-risk bot behavior.
        */
        public static BotStrategySettings Conservative()
        {
            // Stands earlier, less aggressive.
            return new BotStrategySettings(hitUntilValue: 15, allowDoubleDown: false);
        }

        /*
         Standard()
         - Preset settings representing a balanced/default bot:
           * Hits up to 16.
           * Allows double down.
        */
        public static BotStrategySettings Standard()
        {
            return new BotStrategySettings(hitUntilValue: 16, allowDoubleDown: true);
        }

        /*
         Aggressive()
         - Preset settings for a higher-risk bot:
           * Hits up to 17 (takes more risks).
           * Allows double down.
         - Useful when testing more volatile play styles.
        */
        public static BotStrategySettings Aggressive()
        {
            // Hits longer, higher risk.
            return new BotStrategySettings(hitUntilValue: 17, allowDoubleDown: true);
        }
    }
}
