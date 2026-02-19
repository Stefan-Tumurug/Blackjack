using Blackjack.Core.Domain;
using Blackjack.Core.Players;

namespace Blackjack.Core.Abstractions
{
    public interface IGameObserver
    {
        void OnRoundStarted();
        void OnDealerDealt(Card upCard, Card holeCard);
        void OnPlayerDealt(Player player, PlayerHand hand, Card card);
        void OnPlayerDecision(Player player, PlayerHand hand, string decision);
        void OnPlayerCardDrawn(Player player, PlayerHand hand, Card card);
        void OnDealerCardDrawn(Card card, int dealerValue);
    }

    public sealed class NullGameObserver : IGameObserver
    {
        public void OnRoundStarted() { }
        public void OnDealerDealt(Card upCard, Card holeCard) { }
        public void OnPlayerDealt(Player player, PlayerHand hand, Card card) { }
        public void OnPlayerDecision(Player player, PlayerHand hand, string decision) { }
        public void OnPlayerCardDrawn(Player player, PlayerHand hand, Card card) { }
        public void OnDealerCardDrawn(Card card, int dealerValue) { }
    }
}
