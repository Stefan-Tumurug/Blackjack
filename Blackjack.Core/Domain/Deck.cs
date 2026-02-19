using Blackjack.Core.Abstractions;
using System;
using System.Collections.Generic;

namespace Blackjack.Core.Domain
{
    // Deck
    // - Represents a shuffled pile of playing cards used by the game engine.
    // - Responsibility:
    //     * Construct a standard 52-card deck (no jokers).
    //     * Shuffle cards using a Fisher–Yates algorithm.
    //     * Provide a single-card Draw operation and expose the remaining Count.
    // - Notes:
    //     * A deterministic seed can be supplied to the constructor to make shuffle order
    //       reproducible for tests.
    //     * The class is intentionally simple and focused on card ordering; it does not
    //       implement advanced behaviors like multiple shoe handling or automatic reshuffle.
    public sealed class Deck : IDeck
    {
        private readonly List<Card> _cards;
        private readonly Random _random;

        // seed is useful for tests: the same seed produces the same shuffle order.
        public Deck(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            _cards = CreateStandard52CardDeck();
            Shuffle();
        }

        // Number of cards remaining in the deck.
        // Useful for diagnostics and tests that assert deck exhaustion.
        public int Count => _cards.Count;

        // Draws the top card from the deck. Throws if the deck is empty.
        // Behavior:
        //  - Removes and returns the card at index 0 (top of the deck).
        //  - Throws InvalidOperationException when called with an empty deck to make callers
        //    explicitly handle the exhausted deck case.
        public Card Draw()
        {
            if (_cards.Count == 0)
                throw new InvalidOperationException("Cannot draw from an empty deck.");
            Card topCard = _cards[0];
            _cards.RemoveAt(0);
            return topCard;
        }

        // Fisher–Yates shuffle algorithm for efficient, unbiased shuffling.
        // Implementation details:
        //  - Iterates backwards and swaps each element with a random earlier element (including itself).
        //  - Uses the instance Random so providing a seed yields reproducible shuffles.
        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _random.Next(0, i + 1);
                Card temp = _cards[i];
                _cards[i] = _cards[j];
                _cards[j] = temp;
            }
        }

        // Creates a standard 52-card deck (no jokers).
        // Uses Enum.GetValues to iterate all suits and ranks defined in Card.cs.
        private static List<Card> CreateStandard52CardDeck()
        {
            List<Card> cards = new List<Card>(capacity: 52);

            foreach (Suit suit in Enum.GetValues<Suit>())
            {
                foreach (Rank rank in Enum.GetValues<Rank>())
                {
                    cards.Add(new Card(suit, rank));
                }
            }
            return cards;
        }
    }
}
