using Blackjack.Core.Domain;
using Blackjack.Core.Players;

namespace Blackjack.Core.Game
{
    // Data object passed into a strategy to make a decision.
    // Contains enough information to decide for one specific hand.
    public sealed class PlayerDecisionContext
    {
        public PlayerHand PlayerHand { get; }
        public Card DealerUpCard { get; }
        public bool CanDoubleDown { get; }
        public bool CanSplit { get; }

        public PlayerDecisionContext(
            PlayerHand playerHand,
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
