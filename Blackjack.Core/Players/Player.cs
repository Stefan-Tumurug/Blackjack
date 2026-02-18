using System;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;

namespace Blackjack.Core.Players
{
    // Represents one participant in the game.
    // Holds the player's money and current hand for the round.
    public sealed class Player
    {
        public string Name { get; }
        public Bankroll Bankroll { get; }
        public Bet? CurrentBet { get; private set; }
        public Hand Hand { get; }
        public PlayerHandState HandState { get; }
        public IPlayerStrategy Strategy { get; }

        public Player(string name, Bankroll bankroll, IPlayerStrategy strategy)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A name is required", nameof(name));
            }

            Name = name;
            Bankroll = bankroll ?? throw new ArgumentNullException(nameof(bankroll));
            Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

            Hand = new Hand();
            HandState = new PlayerHandState();
        }

        public void SetBet(Bet bet)
        {
            if (bet == null)
            {
                throw new ArgumentNullException(nameof(bet));
            }

            if (!Bankroll.CanPlaceBet(bet.Amount))
            {
                throw new InvalidOperationException("Bet is not allowed for this bankroll.");
            }

            CurrentBet = bet;
        }

        public void ResetForNewRound()
        {
            // Resets state so the same Player instance can be reused across rounds.
            CurrentBet = null;
            Hand.Clear();
            HandState.Reset();
        }
    }
}
