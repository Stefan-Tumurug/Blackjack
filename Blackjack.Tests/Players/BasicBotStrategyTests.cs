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
            Hand hand = new Hand();
            hand.AddCard(new Card(Suit.Clubs, Rank.Ten));  // 10
            hand.AddCard(new Card(Suit.Clubs, Rank.Six));  // 16

            PlayerDecisionContext context = new PlayerDecisionContext(
                playerHand: hand,
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
            Hand hand = new Hand();
            hand.AddCard(new Card(Suit.Clubs, Rank.Ten));   // 10
            hand.AddCard(new Card(Suit.Clubs, Rank.Seven)); // 17

            PlayerDecisionContext context = new PlayerDecisionContext(
                playerHand: hand,
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
            Hand hand = new Hand();
            hand.AddCard(new Card(Suit.Clubs, Rank.Five)); // 5
            hand.AddCard(new Card(Suit.Clubs, Rank.Six));  // 11

            PlayerDecisionContext context = new PlayerDecisionContext(
                playerHand: hand,
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
            Hand hand = new Hand();
            hand.AddCard(new Card(Suit.Clubs, Rank.Five));
            hand.AddCard(new Card(Suit.Clubs, Rank.Six));  // 11

            PlayerDecisionContext context = new PlayerDecisionContext(
                playerHand: hand,
                dealerUpCard: new Card(Suit.Spades, Rank.Six),
                canDoubleDown: true,
                canSplit: false);

            // Act
            PlayerDecision decision = strategy.Decide(context);

            // Assert
            Assert.AreNotEqual(PlayerDecision.DoubleDown, decision);
        }
    }
}
