using System;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;
using Blackjack.Core.Players;
namespace Blackjack.Core.Game
{
    // Coordinates one blackjack round for multiple players against a dealer.
    // Contains no UI logic and depends only on abstractions.
    /*
     GameEngine
     - Responsible for driving a single round's lifecycle:
       * Starting the round and dealing initial cards
       * Playing each player's hands (including hit/stand/double/split)
       * Executing dealer play according to dealer rules
       * Resolving winners for each hand and applying payouts
     - Designed to be UI-agnostic: it uses IGameObserver to report events and IPayoutCalculator
       to compute settlement amounts.
     - Consumers must ensure valid inputs (e.g., non-empty player list, deck with enough cards).
    */
    public sealed class GameEngine
    {
        private readonly IDeck _deck;
        private readonly IPayoutCalculator _payoutCalculator;
        private readonly IGameObserver _observer;

        public IReadOnlyList<Player> Players { get; }
        public Hand DealerHand { get; }

        public GameEngine(
            IDeck deck,
            IPayoutCalculator payoutCalculator,
            IReadOnlyList<Player> players,
            IGameObserver? observer = null)
        {
            _deck = deck ?? throw new ArgumentNullException(nameof(deck));
            _payoutCalculator = payoutCalculator ?? throw new ArgumentNullException(nameof(payoutCalculator));
            _observer = observer ?? new NullGameObserver();
            Players = players ?? throw new ArgumentNullException(nameof(players));

            if (Players.Count == 0)
            {
                throw new ArgumentException("At least one player is required.", nameof(players));
            }

            DealerHand = new Hand();
        }

        /*
         StartRound
         - Prepares engine state for a new round:
           * Clears dealer's hand
           * Notifies observers via OnRoundStarted
           * Verifies each player has at least one hand allocated (caller responsibility)
           * Performs the initial deal of two cards to dealer and two cards to each player's first hand
         - Throws when a player has no hands configured.
        */
        public void StartRound()
        {
            DealerHand.Clear();
            _observer.OnRoundStarted();

            foreach (Player player in Players)
            {
                if (player.Hands.Count == 0)
                {
                    throw new InvalidOperationException($"Player {player.Name} has no hands for the round.");
                }
            }

            DealInitialCards();
        }

        private void DealInitialCards()
        {
            Card upCard = _deck.Draw();
            Card holeCard = _deck.Draw();

            DealerHand.AddCard(upCard);
            DealerHand.AddCard(holeCard);

            _observer.OnDealerDealt(upCard, holeCard);

            foreach (Player player in Players)
            {
                PlayerHand firstHand = player.Hands[0];

                Card firstCard = _deck.Draw();
                firstHand.Hand.AddCard(firstCard);
                _observer.OnPlayerDealt(player, firstHand, firstCard);

                Card secondCard = _deck.Draw();
                firstHand.Hand.AddCard(secondCard);
                _observer.OnPlayerDealt(player, firstHand, secondCard);
            }
        }

        /*
         PlayPlayers
         - Iterates each player and every PlayerHand they own and executes player actions.
         - The dealer's visible up card is provided to strategies via PlayerDecisionContext.
         - Each hand is played independently, allowing split-created hands to be handled by the
           outer iteration when discovered.
        */
        public void PlayPlayers()
        {
            Card dealerUpCard = DealerHand.Cards[0];

            foreach (Player player in Players)
            {
                for (int handIndex = 0; handIndex < player.Hands.Count; handIndex++)
                {
                    PlayerHand playerHand = player.Hands[handIndex];
                    PlaySingleHand(player, playerHand, dealerUpCard);
                }
            }
        }

        /*
         DealerPlay
         - Executes the dealer's turn using the standard "draw until at least 17" rule.
         - After each drawn card the observer is notified with the current dealer value.
         - Assumes Hand.GetValue() handles Ace (soft/hard) calculations.
         - If the deck runs out, Deck.Draw() will throw — callers should ensure a sufficiently sized deck.
        */
        public void DealerPlay()
        {
            // Dealer draws until at least 17 (standard rule).
            while (DealerHand.GetValue() < 17)
            {
                Card drawn = _deck.Draw();
                DealerHand.AddCard(drawn);

                int dealerValue = DealerHand.GetValue();
                _observer.OnDealerCardDrawn(drawn, dealerValue);
            }
        }

        /*
         PlaySingleHand
         - Core loop that runs a single PlayerHand through decisions until stand, bust or completion.
         - Uses the player's IPlayerStrategy to obtain a PlayerDecision via PlayerDecisionContext.
         - Supports Hit, Stand, DoubleDown and Split with validation methods:
           * CanDoubleDown - checks two-card constraint, doubled status and bankroll
           * CanSplit - checks two-card equal-rank constraint, bankroll and single-split rule
         - Observers are called for decisions and card draws to allow UI or logging.
         - Invalid or unsupported decisions fall back to Stand to keep engine robust.
        */
        private void PlaySingleHand(Player player, PlayerHand playerHand, Card dealerUpCard)
        {
            while (true)
            {
                if (playerHand.Hand.IsBust)
                {
                    return;
                }

                PlayerDecision decision = player.Strategy.Decide(
                    new PlayerDecisionContext(
                        playerHand,
                        dealerUpCard,
                        canDoubleDown: CanDoubleDown(player, playerHand),
                        canSplit: CanSplit(playerHand, player)));
                _observer.OnPlayerDecision(player, playerHand, decision.ToString());


                if (decision == PlayerDecision.Stand)
                {
                    playerHand.State.Stand();
                    return;
                }

                if (decision == PlayerDecision.Hit)
                {
                    Card drawn = _deck.Draw();
                    playerHand.Hand.AddCard(drawn);
                    _observer.OnPlayerCardDrawn(player, playerHand, drawn);
                    continue;
                }


                if (decision == PlayerDecision.DoubleDown && CanDoubleDown(player, playerHand))
                {
                    Card drawn = _deck.Draw();
                    playerHand.Hand.AddCard(drawn);
                    _observer.OnPlayerCardDrawn(player, playerHand, drawn);

                    playerHand.State.MarkDoubledDown();
                    return;
                }


                if (decision == PlayerDecision.Split && CanSplit(playerHand, player))
                {
                    _observer.OnPlayerDecision(player, playerHand, "Split");
                    PerformSplit(player, playerHand);
                    // After splitting, continue playing the current hand.
                    // The new hand will be played later by the outer loop.
                    continue;
                }

                // If strategy returns an invalid decision, default to stand.
                playerHand.State.Stand();
                return;
            }
        }

        /*
         CanDoubleDown
         - Determines whether the player may double down on the provided hand.
         - Rules enforced:
           * Only allowed when the hand currently contains exactly two cards.
           * The hand must not have been doubled previously.
           * The player's bankroll must be sufficient to cover the doubled stake (worst-case loss = 2x bet).
         - This check is conservative and intentionally prevents negative balances during settlement.
        */
        private static bool CanDoubleDown(Player player, PlayerHand playerHand)
        {
            if (playerHand.Hand.Cards.Count != 2)
            {
                return false;
            }

            if (playerHand.State.HasDoubledDown)
            {
                return false;
            }

            // With net settlement, the worst-case loss on double down is 2x bet.
            int worstCaseLoss = playerHand.Bet.Amount * 2;
            return player.Bankroll.Balance >= worstCaseLoss;
        }

        /*
         CanSplit
         - Minimal split validation:
           * Only allowed on exactly two cards.
           * Ranks of the two cards must match.
           * Player may have at most one split (no resplitting in this implementation).
           * Player's bankroll must be sufficient to cover the additional bet (worst-case 2x bet).
         - Returns true when a split is permitted; false otherwise.
        */
        private static bool CanSplit(PlayerHand playerHand, Player player)
        {
            // Minimal split rule:
            // - only allowed on exactly two cards
            // - ranks must match
            // - only one split per original player (no re-splitting)
            if (playerHand.Hand.Cards.Count != 2)
            {
                return false;
            }

            if (player.Hands.Count >= 2)
            {
                return false;
            }

            int worstCaseLoss = playerHand.Bet.Amount * 2;
            if (player.Bankroll.Balance < worstCaseLoss)
            {
                return false;
            }

            Card first = playerHand.Hand.Cards[0];
            Card second = playerHand.Hand.Cards[1];

            return first.Rank == second.Rank;
        }

        /*
         PerformSplit
         - Executes a split operation when allowed:
           * Validates bankroll again and rejects if insufficient funds.
           * Creates a new PlayerHand with the same bet as the original.
           * Moves the second card into the new hand and leaves the original with the first card.
           * Deals one new card to each hand from the deck.
           * Adds the created hand to the player's Hands collection.
         - Notes:
           * This method keeps checks conservative and returns without modifying state when split is not possible.
           * The method assumes PlayerHand, Player and Deck behave as expected (no concurrent modifications).
        */
        private void PerformSplit(Player player, PlayerHand originalHand)
        {
            // Splitting duplicates the bet for the new hand.
            // We keep bankroll checks simple: require bankroll can cover the same bet again.
            if (!player.Bankroll.CanPlaceBet(originalHand.Bet.Amount))
            {
                // Not enough money to split. Strategy asked for it, but we reject it.
                return;
            }

            int worstCaseLoss = originalHand.Bet.Amount * 2;
            if (player.Bankroll.Balance < worstCaseLoss)
            {
                return;
            }

            // Create a new hand with the same bet.
            PlayerHand splitHand = new PlayerHand(originalHand.Bet);

            // Move one card from the original hand to the split hand.
            Card movedCard = originalHand.Hand.Cards[1];

            // Rebuild original hand: keep first card only.
            // We don't expose mutation of Cards, so we reset and re-add.
            Card firstCard = originalHand.Hand.Cards[0];

            originalHand.Hand.Clear();
            originalHand.Hand.AddCard(firstCard);

            splitHand.Hand.AddCard(movedCard);

            // Deal one new card to each hand after splitting.
            originalHand.Hand.AddCard(_deck.Draw());
            splitHand.Hand.AddCard(_deck.Draw());

            player.Hands.Add(splitHand);
        }

        /*
         ResolveResults
         - Evaluates each player's hand against the dealer and returns a list of result tuples
           containing a PlayerHandKey and the resolved RoundResult.
         - Uses DetermineWinner to decide each individual hand outcome.
        */
        public IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> ResolveResults()
        {
            List<(PlayerHandKey, RoundResult)> results = new();

            foreach (Player player in Players)
            {
                foreach (PlayerHand hand in player.Hands)
                {
                    RoundResult result = DetermineWinner(hand.Hand, DealerHand);
                    results.Add((new PlayerHandKey(player, hand), result));
                }
            }

            return results;
        }

        /*
         ApplyPayouts
         - Applies payouts to each player according to the provided results.
         - For each resolved hand:
           * Compute net change using IPayoutCalculator.CalculateNetChange (base bet, result, doubledDown)
           * Apply the returned net change to the player's Bankroll via ApplyNetChange
         - This method centralizes settlement so the engine does not directly calculate money rules.
        */
        public void ApplyPayouts(IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results)
        {
            foreach ((PlayerHandKey key, RoundResult result) in results)
            {
                int net = _payoutCalculator.CalculateNetChange(
                    key.Hand.Bet.Amount,
                    result,
                    key.Hand.State.HasDoubledDown);

                key.Player.Bankroll.ApplyNetChange(net);
            }
        }

        /*
         DetermineWinner
         - Compares a single player's hand against the dealer to decide the RoundResult.
         - Rules:
           * If the player's hand is bust => DealerWin
           * If the dealer's hand is bust => PlayerWin
           * Otherwise compare numeric values: higher value wins, equal values => Push
         - Uses Hand's GetValue() and IsBust which include Ace handling.
        */
        private static RoundResult DetermineWinner(Hand playerHand, Hand dealerHand)
        {
            if (playerHand.IsBust) return RoundResult.DealerWin;
            if (dealerHand.IsBust) return RoundResult.PlayerWin;

            int playerValue = playerHand.GetValue();
            int dealerValue = dealerHand.GetValue();

            if (playerValue > dealerValue) return RoundResult.PlayerWin;
            if (playerValue < dealerValue) return RoundResult.DealerWin;

            return RoundResult.Push;
        }
    }
}