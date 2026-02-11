using Blackjack.Core.Domain;
using Blackjack.Core.Game;
using Blackjack.Cli.UI;

while (true)
{
    Console.Clear();

    // Composition root: CLI creates concrete dependencies
    Deck deck = new Deck();
    GameEngine engine = new GameEngine(deck);

    engine.StartRound();

    // Players turn
    while (true)
    {
        ConsoleRenderer.ShowHands(engine.PlayerHand, engine.DealerHand, hideDealerHoleCard: true);

        if (engine.PlayerHand.IsBust)
        {
            ConsoleRenderer.ShowResult("You busted! Dealer wins.");
            break;
        }

        int choice = ConsoleInput.ReadMenuChoice("Vælg: (1) Hit, (2) Stand, (0) Exit: ", 0, 1, 2);

        if (choice == 1)
        {
            engine.PlayerHit();
            continue;
        }

        // Stand
        break;
    }

    // Dealer plays if player hasn't busted already
    if (!engine.PlayerHand.IsBust)
    {
        engine.DealerPlay();

        ConsoleRenderer.ShowHands(engine.PlayerHand, engine.DealerHand, hideDealerHoleCard: false);

        RoundResult result = engine.DetermineWinner();
        string text = result switch
        {
            RoundResult.PlayerWin => "You win!",
            RoundResult.DealerWin => "Dealer wins.",
            _ => "Draw (push)."
        };

        ConsoleRenderer.ShowResult(text);
    }

    int again = ConsoleInput.ReadMenuChoice("Play again? 1 = Yes, 0 = No: ", 1, 0);
    if (again == 0)
        break;
}


