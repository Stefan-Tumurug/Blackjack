using Blackjack.Core.Domain;

namespace Blackjack.Core.Abstractions;

// Abstraction for decks used by the engine.
// - Allows swapping in test/mock decks for deterministic unit tests.
// - Keeps production code dependent only on the minimal operations required by the game logic.
// Implementations are expected to represent a finite collection of cards and provide a single
// draw operation that removes and returns the next card.
public interface IDeck
{
    // Number of cards remaining in the deck.
    // Useful for diagnostics, reshuffle logic or tests that assert deck exhaustion.
    int Count { get; }

    // Draws the next card from the deck.
    // Implementations should remove the returned card from their internal collection.
    // Callers can assume this method returns a non-null Card when Count > 0.
    Card Draw();
}
