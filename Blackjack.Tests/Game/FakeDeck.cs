using System;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Domain;

namespace Blackjack.Tests.Game;

// Fake deck til tests: returnerer kort i en forudbestemt rækkefølge.
// Gør tests deterministiske og uafhængige af shuffling.

public sealed class FakeDeck : IDeck
{
    private readonly Queue<Card> _cards;
    public FakeDeck(IEnumerable<Card> cards)
    {
        if (cards == null)
            throw new ArgumentNullException(nameof(cards));
        _cards = new Queue<Card>(cards);
    }
    public int Count => _cards.Count;
    public Card Draw()
    {
        if (_cards.Count == 0)
            throw new InvalidOperationException("Cannot draw from an empty deck.");
        return _cards.Dequeue();
    }
}
