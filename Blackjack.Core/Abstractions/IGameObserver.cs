using Blackjack.Core.Domain;
using Blackjack.Core.Players;

namespace Blackjack.Core.Abstractions;

// Observer abstraction for reporting game events to external listeners (UI, tests, logging, etc.).
/*
 IGameObserver
 - A lightweight callback-style interface that the game engine uses to report
   important events during a round. Implementations can render a UI, collect
   telemetry, or remain empty for tests.
 - Designed to keep the engine UI-agnostic: the engine only raises events, it does
   not know how observers will present or process them.
 - Methods are synchronous and are called from the engine's play loop.
*/
public interface IGameObserver
{
    // Called at the beginning of each round so observers can reset UI state or buffers.
    void OnRoundStarted();

    // Called after the dealer receives their initial two cards.
    // - upCard: visible card shown to players.
    // - holeCard: dealer's hidden card (observers may choose to hide it).
    void OnDealerDealt(Card upCard, Card holeCard);

    // Called each time a player is dealt a card.
    // - player: the player receiving the card.
    // - hand: the specific PlayerHand instance that received the card (useful for splits).
    // - card: the card that was dealt.
    void OnPlayerDealt(Player player, PlayerHand hand, Card card);

    // Called when a player makes a decision (e.g. "Hit", "Stand", "Double", "Split").
    // - decision is a human-readable representation; observers can localize or format as needed.
    void OnPlayerDecision(Player player, PlayerHand hand, string decision);

    // Called when a player draws an additional card after the initial deal.
    void OnPlayerCardDrawn(Player player, PlayerHand hand, Card card);

    // Called when the dealer draws a card during dealer play.
    // - dealerValue: the dealer's hand value after drawing the card (convenience for display).
    void OnDealerCardDrawn(Card card, int dealerValue);
}

// Minimal "no-op" observer implementation that can be used when no observer behavior is needed.
/*
 NullGameObserver
 - Provides empty implementations for all observer callbacks.
 - Useful as a default to avoid null checks in code that accepts an IGameObserver.
*/
public sealed class NullGameObserver : IGameObserver
{
    public void OnRoundStarted() { }
    public void OnDealerDealt(Card upCard, Card holeCard) { }
    public void OnPlayerDealt(Player player, PlayerHand hand, Card card) { }
    public void OnPlayerDecision(Player player, PlayerHand hand, string decision) { }
    public void OnPlayerCardDrawn(Player player, PlayerHand hand, Card card) { }
    public void OnDealerCardDrawn(Card card, int dealerValue) { }
}
