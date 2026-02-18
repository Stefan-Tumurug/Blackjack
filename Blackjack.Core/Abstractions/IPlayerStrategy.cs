using Blackjack.Core.Game;

namespace Blackjack.Core.Abstractions
{
    // Strategy pattern: lets GameEngine ask "how should this player act?"
    // without depending on UI (CLI/GUI) or on a specific bot implementation.
    public interface IPlayerStrategy
    {
        PlayerDecision Decide(PlayerDecisionContext context);
    }
}
