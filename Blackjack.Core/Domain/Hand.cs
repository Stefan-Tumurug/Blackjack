using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Core.Domain
{
    // En hånd repræsenterer de kort, som spiller eller dealer har på handen.
    // Denne klasse indeholder regelen for korrekt beregning af en håndværdi med Es.
    public sealed class Hand
    {
        private readonly List<Card> _cards = new List<Card>();
        public IReadOnlyList<Card> Cards => _cards;
        public void AddCard(Card card)
        {
               _cards.Add(card);
        }

        // Regler:
        // - Billedkort = 10 (ligger i Card.Value)
        // - Es = 11 eller 1 (justeres i denne metode)
        // Vi starter med Es = 11 og "nedgraderer" til 1 hvis totalen overstiger 21. (bust)
        public int GetValue()
        {
            int total = _cards.Sum(c => c.Value);
            int aceCount = _cards.Count(c => c.IsAce);

            // Hvis total > 21, kan vi redde h[nden ved at gøre et eller flere Es til 1.
            // Hvert Es der nedgraderes reducerer total med 10 (fra 11 til 1).
            while (total > 21 && aceCount > 0)
            {
                total -= 10;
                aceCount--;
            }
            return total;
        }

        // Convenience property til game flow, så vi kan stoppe runden ved bust.
        public bool IsBust => GetValue() > 21;
    }
}
