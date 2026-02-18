using Blackjack.Core.Domain;

namespace Blackjack.Core.Game
{
    // Data object passed into a strategy to make a decision.
    // This keeps the strategy API stable and testable.
    public sealed class PlayerDecisionContext
    {
        public Hand PlayerHand { get; }
        public Card DealerUpCard { get; }
        public bool CanDoubleDown { get; }
        public bool CanSplit { get; }

        public PlayerDecisionContext(
            Hand playerHand,
            Card dealerUpCard,
            bool canDoubleDown,
            bool canSplit)
        {
            PlayerHand = playerHand;
            DealerUpCard = dealerUpCard;
            CanDoubleDown = canDoubleDown;
            CanSplit = canSplit;
        }
    }
}
