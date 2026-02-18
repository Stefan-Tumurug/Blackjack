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

            Player p1 = CreatePlayer("P1", 100, 10, new AlwaysStandStrategy());
            Player p2 = CreatePlayer("P2", 100, 10, new AlwaysStandStrategy());

            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1, p2 });

            // Act
            engine.StartRound();

            // Assert
            Assert.HasCount(2, p1.Hand.Cards);
            Assert.HasCount(2, p2.Hand.Cards);
            Assert.HasCount(2, engine.DealerHand.Cards);
        }

        [TestMethod]
        public void DealerPlay_DrawsUntilAtLeast17()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // One player start
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Clubs, Rank.Ten),

                // Dealer start = 6 + 6 = 12
                new Card(Suit.Spades, Rank.Six),
                new Card(Suit.Spades, Rank.Six),

                // Dealer draws: +5 => 17
                new Card(Suit.Hearts, Rank.Five)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", 100, 10, new AlwaysStandStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();

            // Act
            engine.DealerPlay();

            // Assert
            Assert.IsGreaterThanOrEqualTo(17, engine.DealerHand.GetValue());
        }

        [TestMethod]
        public void ResolveResults_ReturnsResultForEachPlayer()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // P1 = 20
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Clubs, Rank.Ten),

                // P2 = 18
                new Card(Suit.Diamonds, Rank.Ten),
                new Card(Suit.Diamonds, Rank.Eight),

                // Dealer = 19
                new Card(Suit.Spades, Rank.Ten),
                new Card(Suit.Spades, Rank.Nine)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", 100, 10, new AlwaysStandStrategy());
            Player p2 = CreatePlayer("P2", 100, 10, new AlwaysStandStrategy());

            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1, p2 });

            engine.StartRound();

            // Act
            IReadOnlyDictionary<Player, RoundResult> results = engine.ResolveResults();

            // Assert
            Assert.HasCount(2, results);
            Assert.IsTrue(results.ContainsKey(p1));
            Assert.IsTrue(results.ContainsKey(p2));
        }

        [TestMethod]
        public void ApplyPayouts_PlayerWin_IncreasesBankroll()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // Player = 20
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Clubs, Rank.Ten),

                // Dealer = 19
                new Card(Suit.Spades, Rank.Ten),
                new Card(Suit.Spades, Rank.Nine)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", 100, 10, new AlwaysStandStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();

            IReadOnlyDictionary<Player, RoundResult> results = engine.ResolveResults();

            // Act
            engine.ApplyPayouts(results);

            // Assert
            Assert.AreEqual(110, p1.Bankroll.Balance);
        }

        [TestMethod]
        public void ApplyPayouts_DealerWin_DecreasesBankroll()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // Player = 18
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Clubs, Rank.Eight),

                // Dealer = 19
                new Card(Suit.Spades, Rank.Ten),
                new Card(Suit.Spades, Rank.Nine)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", 100, 10, new AlwaysStandStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();

            IReadOnlyDictionary<Player, RoundResult> results = engine.ResolveResults();

            // Act
            engine.ApplyPayouts(results);

            // Assert
            Assert.AreEqual(90, p1.Bankroll.Balance);
        }

        [TestMethod]
        public void ApplyPayouts_Push_DoesNotChangeBankroll()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // Player = 19
                new Card(Suit.Clubs, Rank.Ten),
                new Card(Suit.Clubs, Rank.Nine),

                // Dealer = 19
                new Card(Suit.Spades, Rank.Ten),
                new Card(Suit.Spades, Rank.Nine)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", 100, 10, new AlwaysStandStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();

            IReadOnlyDictionary<Player, RoundResult> results = engine.ResolveResults();

            // Act
            engine.ApplyPayouts(results);

            // Assert
            Assert.AreEqual(100, p1.Bankroll.Balance);
        }

        [TestMethod]
        public void ApplyPayouts_DoubleDown_Win_PaysDouble()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                // Player start = 11 (5 + 6), then double down draws +10 => 21
                new Card(Suit.Clubs, Rank.Five),
                new Card(Suit.Clubs, Rank.Six),

                // Dealer start = 20 (10 + 10)
                new Card(Suit.Spades, Rank.Ten),
                new Card(Suit.Spades, Rank.Ten),

                // Player double down draw
                new Card(Suit.Hearts, Rank.Ten)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();
            Player p1 = CreatePlayer("P1", 100, 10, new AlwaysDoubleDownIfPossibleStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            engine.StartRound();

            // Act
            engine.PlayPlayers(); // triggers double down
            IReadOnlyDictionary<Player, RoundResult> results = engine.ResolveResults();
            engine.ApplyPayouts(results);

            // Assert
            // Player wins and doubled down => +20
            Assert.AreEqual(120, p1.Bankroll.Balance);
        }

        [TestMethod]
        public void StartRound_WithoutBet_Throws()
        {
            // Arrange
            FakeDeck deck = new FakeDeck(new List<Card>
            {
                new Card(Suit.Clubs, Rank.Two),
                new Card(Suit.Clubs, Rank.Three),
                new Card(Suit.Spades, Rank.Four),
                new Card(Suit.Spades, Rank.Five)
            });

            IPayoutCalculator payout = new StandardPayoutCalculator();

            Player p1 = new Player("P1", new Bankroll(100), new AlwaysStandStrategy());
            GameEngine engine = new GameEngine(deck, payout, new List<Player> { p1 });

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => engine.StartRound());
        }

        private static Player CreatePlayer(string name, int bankroll, int bet, IPlayerStrategy strategy)
        {
            Player player = new Player(name, new Bankroll(bankroll), strategy);
            player.SetBet(new Bet(bet));
            return player;
        }
    }
}
