using System;
using System.Collections.Generic;

namespace Blackjack.Core.Domain
{
    // A deck represents a shuffled pile of cards.
    // We keep Shuffle and Draw here to separate card logic from UI and game flow.
    public sealed class Deck
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

        public int Count => _cards.Count;

        // Draws the top card from the deck. Throws if the deck is empty.
        public Card Draw()
        {
            if (_cards.Count == 0)
                throw new InvalidOperationException("Cannot draw from an empty deck.");
            Card topCard = _cards[0];
            _cards.RemoveAt(0);
            return topCard;
        }

        // Fisher–Yates shuffle algorithm for efficient, unbiased shuffling.
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
