namespace Blackjack.Core.Game;

/*
 RoundResult
 - Enumerates the possible outcomes for a single player hand after comparing it to the dealer.
 - Used by GameEngine.ResolveResults() to produce per-hand results and by IPayoutCalculator
   implementations to determine settlement amounts.
 - Semantics:
     * PlayerWin  => the player's hand beat the dealer (player should be credited).
     * DealerWin  => the dealer beat the player's hand (player should be debited).
     * Push       => tie; no change to the player's bankroll.
 - Note: Consumers interpret these values; payout logic (including doubled-down stakes)
   is applied by the configured IPayoutCalculator.
*/
public enum RoundResult
{
    PlayerWin,
    DealerWin,
    Push
}
