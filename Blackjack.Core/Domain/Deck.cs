using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack.Core.Domain
{
    // Et deck repræsenterer en blandet kortbunke.
    // Vi holder Shuffle og draw her for at adskille kortlogik fra UI og game flow.
    public sealed class Deck
    {
        private readonly List<Card> _cards;
        private readonly Random _random;

        // seed er nyttig til tests: samme seed giver samme shuffle-rækkefølge.
        public Deck(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            _cards = CreateStandard52CardDeck();
            Shuffle();
        }

        public int Count => _cards.Count;

        // Trækker det øverste kort fra bunken. Kaster exception hvis bunken er tom.
        public Card Draw()
        {
            if (_cards.Count == 0)
                throw new InvalidOperationException("Cannot draw from an empty deck.");
            Card topCard = _cards[0];
            _cards.RemoveAt(0);
            return topCard;
        }

        // Fisher-Yates shuffle algoritme for effektiv og unbiased blanding.
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

        // Opretter et standard 52-korts deck (uden jokere).
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
