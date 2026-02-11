using Blackjack.Core.Domain;

namespace Blackjack.Core.Abstractions;

// Abstraction for mock/test decks.
public interface IDeck
{
    int Count { get; }
    Card Draw();
}
