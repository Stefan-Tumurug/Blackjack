using Blackjack.Core.Abstractions;
using Blackjack.Core.Domain;

namespace Blackjack.Core.Game
{
    // Controls a round of Blackjack.
    // Doesn't involve any UI-logic, gameflow and rules only.
    public sealed class GameEngine
    {
        private readonly IDeck _deck;
        public Hand PlayerHand { get; } = new Hand();
        public Hand DealerHand { get; } = new Hand();

        // Makes it clear that GameEngine requires a deck to run, and throws the excact exception.
        
        public GameEngine(IDeck deck)
        {
            _deck = deck ?? throw new ArgumentNullException(nameof(deck));
        }


        // Startdeal: Starts the game by handing the player and dealer one card each.
        public void StartRound()
        {
            PlayerHand.AddCard(_deck.Draw());
            DealerHand.AddCard(_deck.Draw());

            PlayerHand.AddCard(_deck.Draw());
            DealerHand.AddCard(_deck.Draw());
        }

        public void PlayerHit()
        {
            PlayerHand.AddCard(_deck.Draw());
        }

        public void DealerPlay()
        {
            // Standard blackjack rule:
            // Dealer must draw cards until the value of the hand is 17 or higher.
            while (DealerHand.GetValue() < 17)
            {
                DealerHand.AddCard(_deck.Draw());
            }
        }

        public RoundResult DetermineWinner()
        {
            if (PlayerHand.IsBust)
                return RoundResult.DealerWin;
            if (DealerHand.IsBust)
                return RoundResult.PlayerWin;
            
            int playerValue = PlayerHand.GetValue();
            int dealerValue = DealerHand.GetValue();

            if (playerValue > dealerValue)
                return RoundResult.PlayerWin;

            if (dealerValue > playerValue)
                return RoundResult.DealerWin;

            return RoundResult.Push;
        }
    }
}
