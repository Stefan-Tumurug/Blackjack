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

/*
 Program.cs
 - Entrypoint for the CLI blackjack application using C# top-level statements.
 - Responsible for wiring together domain services (payout calculator), creating persistent
   player instances (so bankroll persists across rounds), and presenting a simple main menu.
 - Contains a synchronous, blocking UI flow intended for human-driven play:
   * Main menu with options to start a game, show tutorial or exit.
   * When starting a game, runs a simple per-round loop that:
     - Shows balances
     - Reads player's bet (validating against bankroll)
     - Lets bots place bets using a simple rule
     - Constructs a new shuffled deck and a GameEngine per round
     - Optionally uses ConsoleGameObserver for step-by-step visualization
     - Runs the round, resolves results, applies payouts and shows round summary
 - Designed for readability and simplicity rather than concurrency or advanced UI features.
*/

IPayoutCalculator payoutCalculator = new StandardPayoutCalculator();

// Create players ONCE so bankroll persists across rounds
string playerName = ConsoleInput.ReadRequiredString("Enter your name: ");

Player player = new Player(
    name: playerName,
    bankroll: new Bankroll(100),
    strategy: new ConsoleHumanStrategy(playerName));

Player botA = new Player(
    name: "Bot A",
    bankroll: new Bankroll(100),
    strategy: new BasicBotStrategy(BotStrategySettings.Standard()));

Player botB = new Player(
    name: "Bot B",
    bankroll: new Bankroll(100),
    strategy: new BasicBotStrategy(BotStrategySettings.Conservative()));

List<Player> players = new List<Player> { player, botA, botB };


// Tutorial session helper (shows a blocking how-to screen)
TutorialSession tutorialSession = new TutorialSession();

// Reset bankroll when starting a new session
player.Bankroll.Reset(100);
botA.Bankroll.Reset(100);
botB.Bankroll.Reset(100);

// Main menu loop
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
        // Show tutorial and return to menu
        tutorialSession.Run();
        continue;
    }

    if (menuChoice == 0)
    {
        // Exit application
        return;
    }

    if (menuChoice == 1)
    {
        player.Bankroll.Reset(100);
        botA.Bankroll.Reset(100);
        botB.Bankroll.Reset(100);
    }


    // Helper to convert RoundResult enum to a human-friendly label for display.
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
    // This inner loop represents a single play session: repeated rounds until the human
    // player chooses to return to the main menu or has no money left.
    while (true)
    {
        Console.Clear();

        if (player.Bankroll.Balance <= 0)
        {
            // Human player broke — inform and return to main menu (break inner loop only)
            Console.Clear();
            Console.WriteLine("You have no money left. Session ended.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey(true);
            break;
        }

        // Show current balances for all players
        Console.WriteLine("=== Balances ===");
        foreach (Player p in players)
        {
            Console.WriteLine($"{p.Name}: {p.Bankroll.Balance}");
        }
        Console.WriteLine();

        // Read player's bet (validated against bankroll)
        Console.WriteLine($"Your balance: {player.Bankroll.Balance}");
        int betAmount = ConsoleInput.ReadIntInRange("Place your bet: ", 1, player.Bankroll.Balance);
        player.StartNewRoundWithBet(new Bet(betAmount));

        // Simple bot betting rule: bet 10% of bankroll (minimum 1)
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

        // Per-round setup: new shuffled deck and game engine
        IDeck deck = new Deck();
        ConsoleGameObserver observer = new ConsoleGameObserver(1200); // visual step delays
        GameEngine engine = new GameEngine(deck, payoutCalculator, players, observer);

        // Play round lifecycle
        engine.StartRound();
        engine.PlayPlayers();
        engine.DealerPlay();

        // Resolve results and apply payouts to bankrolls
        IReadOnlyList<(PlayerHandKey Key, RoundResult Result)> results = engine.ResolveResults();
        engine.ApplyPayouts(results);

        // Show round summary: dealer and each player's hands, values and results
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

        // Prompt to play another round or return to the main menu
        int again = ConsoleInput.ReadMenuChoice("Next round? 1 = Yes, 0 = Back to menu: ", 1, 0);
        if (again == 0)
        {
            break;
        }
        continue;
    }
}
