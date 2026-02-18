using Blackjack.Core.Betting;
using Blackjack.Core.Domain;
using Blackjack.Core.Game;
using Blackjack.Core.Players;
using Blackjack.Core.Players.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blackjack.Tests.Players
{
    [TestClass]
    public sealed class BasicBotStrategyTests
    {
        [TestMethod]
        public void Decide_ValueBelowThreshold_ReturnsHit()
        {
            // Arrange
            BasicBotStrategy strategy = new BasicBotStrategy(BotStrategySettings.Standard());
            PlayerHand playerHand = CreatePlayerHandWithCards(
                betAmount: 10,
                new Card(Suit.Clubs, Rank.Ten),  // 10
                new Card(Suit.Clubs, Rank.Six)); // 16

            PlayerDecisionContext context = new PlayerDecisionContext(
                playerHand: playerHand,
                dealerUpCard: new Card(Suit.Spades, Rank.Two),
                canDoubleDown: false,
                canSplit: false);

            // Act
            PlayerDecision decision = strategy.Decide(context);

            // Assert
            Assert.AreEqual(PlayerDecision.Hit, decision);
        }

        [TestMethod]
        public void Decide_ValueAboveThreshold_ReturnsStand()
        {
            // Arrange
            BasicBotStrategy strategy = new BasicBotStrategy(BotStrategySettings.Standard());
            PlayerHand playerHand = CreatePlayerHandWithCards(
                betAmount: 10,
                new Card(Suit.Clubs, Rank.Ten),   // 10
                new Card(Suit.Clubs, Rank.Seven)); // 17

            PlayerDecisionContext context = new PlayerDecisionContext(
                playerHand: playerHand,
                dealerUpCard: new Card(Suit.Spades, Rank.Two),
                canDoubleDown: false,
                canSplit: false);

            // Act
            PlayerDecision decision = strategy.Decide(context);

            // Assert
            Assert.AreEqual(PlayerDecision.Stand, decision);
        }

        [TestMethod]
        public void Decide_CanDoubleDown_AndValueIs10Or11_ReturnsDoubleDown()
        {
            // Arrange
            BasicBotStrategy strategy = new BasicBotStrategy(BotStrategySettings.Standard());
            PlayerHand playerHand = CreatePlayerHandWithCards(
                betAmount: 10,
                new Card(Suit.Clubs, Rank.Five), // 5
                new Card(Suit.Clubs, Rank.Six)); // 11

            PlayerDecisionContext context = new PlayerDecisionContext(
                playerHand: playerHand,
                dealerUpCard: new Card(Suit.Spades, Rank.Six),
                canDoubleDown: true,
                canSplit: false);

            // Act
            PlayerDecision decision = strategy.Decide(context);

            // Assert
            Assert.AreEqual(PlayerDecision.DoubleDown, decision);
        }

        [TestMethod]
        public void Decide_DoubleDownNotAllowed_ReturnsHitOrStand()
        {
            // Arrange
            BasicBotStrategy strategy = new BasicBotStrategy(BotStrategySettings.Conservative());
            PlayerHand playerHand = CreatePlayerHandWithCards(
                betAmount: 10,
                new Card(Suit.Clubs, Rank.Five),
                new Card(Suit.Clubs, Rank.Six)); // 11

            PlayerDecisionContext context = new PlayerDecisionContext(
                playerHand: playerHand,
                dealerUpCard: new Card(Suit.Spades, Rank.Six),
                canDoubleDown: true,
                canSplit: false);

            // Act
            PlayerDecision decision = strategy.Decide(context);

            // Assert
            Assert.AreNotEqual(PlayerDecision.DoubleDown, decision);
        }

        private static PlayerHand CreatePlayerHandWithCards(int betAmount, params Card[] cards)
        {
            PlayerHand playerHand = new PlayerHand(new Bet(betAmount));

            foreach (Card card in cards)
            {
                playerHand.Hand.AddCard(card);
            }

            return playerHand;
        }
    }
}
