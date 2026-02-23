using Blackjack.Core.Domain;
using Blackjack.Core.Players;

namespace Blackjack.Core.Game
{
    /*
     PlayerDecisionContext
     - Immutable container that carries all information a player strategy needs to choose
       an action for a single hand.
     - Keeps decision-time data grouped so strategies remain simple and focused.
     - Fields:
         * PlayerHand    - the specific PlayerHand being played (supports splits).
         * DealerUpCard  - the dealer's visible card (used by strategy heuristics).
         * CanDoubleDown - whether a double-down is currently allowed for this hand.
         * CanSplit      - whether a split is currently allowed for this hand.
     - Note: Validation and game-rule checks are performed by the engine before creating
       this context; strategies should treat the context as authoritative and side-effect free.
    */
    public sealed class PlayerDecisionContext
    {
        // The player's specific hand to act upon.
        public PlayerHand PlayerHand { get; }

        // The dealer's visible up card (first card).
        public Card DealerUpCard { get; }

        // Indicates whether double down is permitted for this hand at this time.
        public bool CanDoubleDown { get; }

        // Indicates whether splitting the hand is permitted at this time.
        public bool CanSplit { get; }

        /*
         Constructor
         - Creates a new context instance with all required decision-time information.
         - All parameters are assigned to readonly properties so the context is effectively immutable.
        */
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
