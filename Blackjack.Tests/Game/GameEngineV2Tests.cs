using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;
using Blackjack.Core.Game;
using Blackjack.Core.Players;
using Blackjack.Tests.Players;
using Blackjack.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Blackjack.Tests.Game
{
    [TestClass]
    public sealed class GameEngineV2Tests
    {
        [TestMethod]
        public void StartRound_DealsTwoCardsToEachPlayer_AndDealerGetsTwoCards()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // P1
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Clubs, Rank.Three),

                // P2
                new Card(Suit.Diamonds, Rank.Four),
                new Card(Suit.Diamonds, Rank.Five),

                // Dealer
                new Card(Suit.Spades, Rank.Six),
                new Card(Suit.Spades, Rank.Seven)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();

            Player p1 = CreatePlayer("P1", bankroll: 100, bet: 10, new AlwaysStandStrategy());
            Player p2 = CreatePlayer("P2", bankroll: 100, bet: 10, new AlwaysStandStrategy());

            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1, p2 });

            // Act
            engine.StartRound();

            // Assert
            Assert.HasCount(2, p1.Hands[0].Hand.Cards);
            Assert.HasCount(2, p2.Hands[0].Hand.Cards);
            Assert.HasCount(2, engine.DealerHand.Cards);
        }

        [TestMethod]
        public void DealerPlay_DrawsUntilAtLeast17()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // Player start
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Clubs, Rank.Ten),

                // Dealer start = 6 + 6 = 12
                new Card(Suit.Spades, Rank.Six),
                new Card(Suit.Spades, Rank.Six),

                // Dealer draws: +5 => 17
                new Card(Suit.Hearts, Rank.Five)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", bankroll: 100, bet: 10, new AlwaysStandStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();

            // Act
            engine.DealerPlay();

            // Assert
            Assert.IsGreaterThanOrEqualTo(17, engine.DealerHand.GetValue());
        }

        [TestMethod]
        public void ResolveResults_ReturnsResultForEachPlayerHand()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // Player start = 10 + 10 = 20
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Clubs, Rank.Ten),

                // Dealer start = 9 + 8 = 17
                new Card(Suit.Spades, Rank.Nine),
                new Card(Suit.Spades, Rank.Eight)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", bankroll: 100, bet: 10, new AlwaysStandStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();
            engine.DealerPlay();

            // Act
            IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results = engine.ResolveResults();

            // Assert
            Assert.HasCount(1, results);
            RoundResult r = FindResultForHand(results, p1, p1.Hands[0]);
            Assert.AreEqual(RoundResult.PlayerWin, r);
        }

        [TestMethod]
        public void ApplyPayouts_PlayerWin_IncreasesBankroll()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // Player start = 20
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Clubs, Rank.Ten),

                // Dealer start = 17
                new Card(Suit.Spades, Rank.Nine),
                new Card(Suit.Spades, Rank.Eight)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", bankroll: 100, bet: 10, new AlwaysStandStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();
            engine.DealerPlay();

            IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results = engine.ResolveResults();

            int before = p1.Bankroll.Balance;

            // Act
            engine.ApplyPayouts(results);

            // Assert
            Assert.IsGreaterThan(before, p1.Bankroll.Balance);
        }

        [TestMethod]
        public void ApplyPayouts_Push_DoesNotChangeBankroll()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // Player = 17
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Clubs, Rank.Seven),

                // Dealer = 17
                new Card(Suit.Spades, Rank.Nine),
                new Card(Suit.Spades, Rank.Eight)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", bankroll: 100, bet: 10, new AlwaysStandStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();
            engine.DealerPlay();

            IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results = engine.ResolveResults();

            int before = p1.Bankroll.Balance;

            // Act
            engine.ApplyPayouts(results);

            // Assert
            Assert.AreEqual(before, p1.Bankroll.Balance);
        }

        [TestMethod]
        public void ApplyPayouts_DoubleDown_Win_PaysDouble()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // Player start = 11 (5+6)
                new Card(Suit.Clubs, Rank.Five),
                new Card(Suit.Clubs, Rank.Six),

                // Dealer start = 17
                new Card(Suit.Spades, Rank.Nine),
                new Card(Suit.Spades, Rank.Eight),

                // Double down card => +10 => 21
                new Card(Suit.Hearts, Rank.Ten)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", bankroll: 100, bet: 10, new AlwaysDoubleDownIfPossibleStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();
            engine.PlayPlayers();
            engine.DealerPlay();

            IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results = engine.ResolveResults();

            int before = p1.Bankroll.Balance;

            // Act
            engine.ApplyPayouts(results);

            // Assert
            Assert.IsGreaterThan(before, p1.Bankroll.Balance);
        }

        private static RoundResult FindResultForHand(
            IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results,
            Player player,
            PlayerHand hand)
        {
            foreach ((PlayerHandKey key, RoundResult result) in results)
            {
                if (ReferenceEquals(key.Player, player) && ReferenceEquals(key.Hand, hand))
                {
                    return result;
                }
            }

            Assert.Fail("Expected to find a result for the given player hand.");
            return RoundResult.Push;
        }

        private static Player CreatePlayer(string name, int bankroll, int bet, IPlayerStrategy strategy)
        {
            Player player = new Player(name, new Bankroll(bankroll), strategy);
            player.StartNewRoundWithBet(new Bet(bet));
            return player;
        }
    }
}
