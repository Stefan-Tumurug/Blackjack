using Blackjack.Cli.Observers;
using Blackjack.Cli.Session;
using Blackjack.Cli.Strategies;
using Blackjack.Cli.UI;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Betting;
using Blackjack.Core.Domain;
using Blackjack.Core.Game;
using Blackjack.Core.Players;
using Blackjack.Core.Players.Strategies;
using System;
using System.Collections.Generic;

IPayoutCalculator payoutCalculator = new StandardPayoutCalculator();

// Create players ONCE so bankroll persists across rounds
Player player = new Player(
    name: "You",
    bankroll: new Bankroll(100),
    strategy: new ConsoleHumanStrategy("You"));

Player botA = new Player(
    name: "Bot A",
    bankroll: new Bankroll(100),
    strategy: new BasicBotStrategy(BotStrategySettings.Standard()));

Player botB = new Player(
    name: "Bot B",
    bankroll: new Bankroll(100),
    strategy: new BasicBotStrategy(BotStrategySettings.Conservative()));

List<Player> players = new List<Player> { player, botA, botB };

// Tutorial session
TutorialSession tutorialSession = new TutorialSession();

// Reset bankroll when starting a new session
player.Bankroll.Reset(100);
botA.Bankroll.Reset(100);
botB.Bankroll.Reset(100);

// Main menu
while (true)
{
    Console.Clear();
    Console.WriteLine("=== Blackjack ===");
    Console.WriteLine("1 - Start Game");
    Console.WriteLine("2 - Tutorial");
    Console.WriteLine("0 - Exit");
    Console.WriteLine();

    int menuChoice = ConsoleInput.ReadMenuChoice("Choose: ", 0, 1, 2);

    if (menuChoice == 2)
    {
        tutorialSession.Run();
        continue;
    }

    if (menuChoice == 0)
    {
        return;
    }
    if (menuChoice == 1)
    {
        // New session starts here
        player.Bankroll.Reset(100);
        botA.Bankroll.Reset(100);
        botB.Bankroll.Reset(100);
    }
    // Format round result for display
    static string FormatResult(RoundResult result)
    {
        return result switch
        {
            RoundResult.PlayerWin => "Win",
            RoundResult.DealerWin => "Lose",
            _ => "Push"
        };
    }


    // ---- GAME LOOP ----
    while (true)
    {
        Console.Clear();

        if (player.Bankroll.Balance <= 0)
        {
            Console.Clear();
            Console.WriteLine("You have no money left. Session ended.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey(true);

            // Break out of the GAME loop only
            break;
        }


        Console.WriteLine("=== Balances ===");
        foreach (Player p in players)
        {
            Console.WriteLine($"{p.Name}: {p.Bankroll.Balance}");
        }
        Console.WriteLine();

        // Bets
        Console.WriteLine($"Your balance: {player.Bankroll.Balance}");
        int betAmount = ConsoleInput.ReadIntInRange("Place your bet: ", 1, player.Bankroll.Balance);
        player.StartNewRoundWithBet(new Bet(betAmount));

        if (botA.Bankroll.Balance > 0)
        {
            int botBet = Math.Max(1, botA.Bankroll.Balance / 10);
            botA.StartNewRoundWithBet(new Bet(botBet));
        }

        if (botB.Bankroll.Balance > 0)
        {
            int botBet = Math.Max(1, botB.Bankroll.Balance / 10);
            botB.StartNewRoundWithBet(new Bet(botBet));
        }

        IDeck deck = new Deck();
        ConsoleGameObserver observer = new ConsoleGameObserver(1200);
        GameEngine engine = new GameEngine(deck, payoutCalculator, players, observer);


        engine.StartRound();
        engine.PlayPlayers();
        engine.DealerPlay();

        IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results = engine.ResolveResults();
        engine.ApplyPayouts(results);

        Console.Clear();
        Console.WriteLine("=== Dealer Hand ===");
        foreach (Card card in engine.DealerHand.Cards)
        {
            Console.WriteLine($" - {card}");
        }
        Console.WriteLine($"Dealer value: {engine.DealerHand.GetValue()}");
        Console.WriteLine();

        foreach (Player p in players)
        {
            Console.WriteLine($"=== {p.Name} (Balance: {p.Bankroll.Balance}) ===");

            for (int i = 0; i < p.Hands.Count; i++)
            {
                PlayerHand hand = p.Hands[i];

                RoundResult handResult = RoundResult.Push;
                foreach ((PlayerHandKey key, RoundResult result) in results)
                {
                    if (ReferenceEquals(key.Player, p) && ReferenceEquals(key.Hand, hand))
                    {
                        handResult = result;
                        break;
                    }
                }

                Console.WriteLine($"Hand #{i + 1} | Bet: {hand.Bet.Amount} | Doubled: {hand.State.HasDoubledDown}");
                foreach (Card card in hand.Hand.Cards)
                {
                    Console.WriteLine($" - {card}");
                }

                string resultText = FormatResult(handResult);
                Console.WriteLine($"Value: {hand.Hand.GetValue()} | Result: {resultText}");
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        int again = ConsoleInput.ReadMenuChoice("Next round? 1 = Yes, 0 = Back to menu: ", 1, 0);
        if (again == 0)
        {
            break;
        }
        continue;
    }
}
