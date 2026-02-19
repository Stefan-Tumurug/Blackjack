using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack.Core.Domain
{
    // Hand
    // - Represents the cards that a player or dealer holds.
    // - Encapsulates blackjack-specific value calculation including correct Ace handling.
    // - Designed to be reused across rounds: Callers should use `Clear()` to reset the hand
    //   instead of allocating a new Hand instance.
    // - Exposes cards as a read-only collection so external code can inspect but not mutate
    //   the internal card list.
    // - Gotchas:
    //     * `GetValue()` recalculates the value on each call and downgrades Aces (11 -> 1) as needed.
    //       Avoid calling it excessively in tight loops if performance is a concern.
    //     * `IsBust` invokes `GetValue()` under the hood, so it also performs the same calculation.
    public sealed class Hand
    {
        private readonly List<Card> _cards = new List<Card>();

        // Public, read-only view of the internal card list.
        // Consumers can enumerate and inspect card instances but cannot modify the Hand's contents.
        public IReadOnlyList<Card> Cards => _cards;

        // Clears the hand for reuse between rounds without allocating a new Hand object.
        public void Clear()
        {
            _cards.Clear();
        }

        // Adds a card to the hand. Throws if `card` is null to preserve invariant.
        public void AddCard(Card card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }   
            _cards.Add(card);
        }

        // GetValue
        // - Computes the best score for the hand using standard blackjack rules:
        //   * Face cards count as 10 (value stored in Card.Value).
        //   * Aces start as 11 but may be downgraded to 1 to avoid busting.
        // - Implementation:
        //   * Start with the sum of all card values (Aces counted as 11).
        //   * While the total exceeds 21 and there are Aces counted as 11, reduce the total by 10
        //     per Ace (effectively converting an 11 into a 1).
        // - Returns the numeric hand value (an integer >= 0).
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

        // Convenience property used by game flow to check for busts.
        // Note: This calls GetValue() and therefore performs the Ace-adjustment logic.
        public bool IsBust => GetValue() > 21;
    }
}
