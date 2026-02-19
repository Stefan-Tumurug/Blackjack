using Blackjack.Core.Game;

namespace Blackjack.Core.Abstractions
{
    // Strategy pattern: allows the GameEngine to ask "how should this player act?"
    // without depending on UI (CLI/GUI) or on a specific bot implementation.
    /*
     IPlayerStrategy
     - A lightweight contract used by the GameEngine to obtain a decision for a single player turn.
     - Implementations can be human-facing (CLI/GUI) or automated bots used in tests and simulations.
     - Implementers should examine the provided PlayerDecisionContext and return the best PlayerDecision
       for the current state of the player's hand and the dealer's up card.
     - Expected characteristics:
       * Synchronous and quick to execute (called from the engine's play loop).
       * Preferably side-effect free: do not modify engine state; if side effects are necessary
         (e.g. logging or UI), keep them local to the strategy implementation.
       * Deterministic for bot strategies to enable reproducible tests.
     - Examples: `ConsoleHumanStrategy` (interactive CLI), `BasicBotStrategy` (automated play logic).
    */
    public interface IPlayerStrategy
    {
        // Decide
        // - Called by the GameEngine to request the player's action for the current PlayerDecisionContext.
        // - The context contains the player's hand, dealer up card and flags such as CanDoubleDown/CanSplit.
        // - Implementations return one of the PlayerDecision values that instructs the engine how to proceed.
        PlayerDecision Decide(PlayerDecisionContext context);
    }
}
