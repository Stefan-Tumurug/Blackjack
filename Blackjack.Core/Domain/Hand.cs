using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Core.Domain
{
    // A hand represents the cards that a player or dealer has.
    // This class contains the rule for correctly calculating a hand value with Aces.
    public sealed class Hand
    {
        private readonly List<Card> _cards = new List<Card>();

        // Expose cards as read-only so callers can inspect the hand
        // without being able to modify internal state.
        public IReadOnlyList<Card> Cards => _cards;
        public void Clear()
        {
            // Resets the hand between rounds without allocating a new Hand instance.
            _cards.Clear();
        }
        public void AddCard(Card card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }
               _cards.Add(card);
        }

        // Rules:
        // - Face cards = 10 (contained in Card.Value)
        // - Aces = 11 or 1 (adjusted in this method)
        // We start with Aces = 11 and "downgrade" to 1 if the total exceeds 21 (bust).
        public int GetValue()
        {
            int total = _cards.Sum(c => c.Value);
            int aceCount = _cards.Count(c => c.IsAce);

            // If total > 21, we can save the hand by converting one or more Aces to 1.
            // Each Ace downgraded reduces the total by 10 (from 11 to 1).
            while (total > 21 && aceCount > 0)
            {
                total -= 10;
                aceCount--;
            }
            return total;
        }

        // Convenience property for game flow, so we can stop the round on a bust.
        public bool IsBust => GetValue() > 21;
    }
}
