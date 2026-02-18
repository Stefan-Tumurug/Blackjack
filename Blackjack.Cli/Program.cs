using System;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;
using Blackjack.Core.Game;
using Blackjack.Core.Players;
using Blackjack.Core.Players.Strategies;
using Blackjack.Cli.UI;

while (true)
{
    Console.Clear();

    IDeck deck = new Deck();
    IPayoutCalculator payoutCalculator = new StandardPayoutCalculator();

    Player player = new Player(
        name: "Player 1",
        bankroll: new Bankroll(100),
        strategy: new BasicBotStrategy(BotStrategySettings.Standard()));

    player.StartNewRoundWithBet(new Bet(10));

    List<Player> players = new List<Player> { player };
    GameEngine engine = new GameEngine(deck, payoutCalculator, players);

    engine.StartRound();
    engine.PlayPlayers();
    engine.DealerPlay();

    IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results = engine.ResolveResults();
    engine.ApplyPayouts(results);

    // Show first hand only for now (split will create a second hand)
    Hand firstHand = player.Hands[0].Hand;

    ConsoleRenderer.ShowHands(firstHand, engine.DealerHand, hideDealerHoleCard: false);

    // Find result for player's first hand
    RoundResult firstHandResult = RoundResult.Push;
    foreach ((PlayerHandKey key, RoundResult result) in results)
    {
        if (ReferenceEquals(key.Player, player) && ReferenceEquals(key.Hand, player.Hands[0]))
        {
            firstHandResult = result;
            break;
        }
    }

    string text = firstHandResult switch
    {
        RoundResult.PlayerWin => "Player wins!",
        RoundResult.DealerWin => "Dealer wins.",
        _ => "Push."
    };

    ConsoleRenderer.ShowResult(text);
    Console.WriteLine($"Balance: {player.Bankroll.Balance}");

    int again = ConsoleInput.ReadMenuChoice("Play again? 1 = Yes, 0 = No: ", 1, 0);
    if (again == 0)
    {
        break;
    }
}
