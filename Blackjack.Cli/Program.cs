using System;
using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;
using Blackjack.Core.Game;
using Blackjack.Core.Players;
using Blackjack.Cli.UI;

while (true)
{
    Console.Clear();

    // Dependencies
    IDeck deck = new Deck();
    IPayoutCalculator payoutCalculator = new StandardPayoutCalculator();

    // Create one human-like bot player for now
    Player player = new Player(
        name: "Player 1",
        bankroll: new Bankroll(100),
        strategy: new BasicBotStrategy());

    player.SetBet(new Bet(10));

    List<Player> players = new List<Player> { player };

    GameEngine engine = new GameEngine(deck, payoutCalculator, players);

    engine.StartRound();

    // Bot plays automatically
    engine.PlayPlayers();

    engine.DealerPlay();

    var results = engine.ResolveResults();
    engine.ApplyPayouts(results);

    ConsoleRenderer.ShowHands(player.Hand, engine.DealerHand, hideDealerHoleCard: false);

    RoundResult result = results[player];

    string text = result switch
    {
        RoundResult.PlayerWin => "Player wins!",
        RoundResult.DealerWin => "Dealer wins.",
        _ => "Push."
    };

    ConsoleRenderer.ShowResult(text);

    Console.WriteLine($"Balance: {player.Bankroll.Balance}");

    int again = ConsoleInput.ReadMenuChoice("Play again? 1 = Yes, 0 = No: ", 1, 0);
    if (again == 0)
        break;
}
