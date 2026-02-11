using Blackjack.Core.Abstractions;
using Blackjack.Core.Domain;

namespace Blackjack.Core.Game
{
    // Styrer én runde Blackjack.
    // Indeholder ingen UI-logik, kun spilflow og regler.
    public sealed class GameEngine
    {
        private readonly IDeck _deck;
        public Hand PlayerHand { get; } = new Hand();
        public Hand DealerHand { get; } = new Hand();

        // Tydeligører at GameEngine kræver et deck for at fungere, og fortæller klart hvilken fejl det er ved crash.
        public GameEngine(IDeck deck)
        {
            _deck = deck ?? throw new ArgumentNullException(nameof(deck));
        }


        // Startdeal: Begynder spillet ved at give både spiller og dealer to kort hver.
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
            // Standard blackjack regel:
            // Dealer skal trække kort indtil hånden er 17 eller højere.
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
