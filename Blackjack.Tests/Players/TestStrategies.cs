using System.Collections.Generic;
using Blackjack.Core.Abstractions;
using Blackjack.Core.Game;

namespace Blackjack.Tests.Players
{
    // Test-only strategies to control GameEngine deterministically.
    internal sealed class AlwaysStandStrategy : IPlayerStrategy
    {
        public PlayerDecision Decide(PlayerDecisionContext context)
        {
            return PlayerDecision.Stand;
        }
    }

    internal sealed class HitThenStandStrategy : IPlayerStrategy
    {
        private readonly int _hitsBeforeStand;
        private int _hitsDone;

        public HitThenStandStrategy(int hitsBeforeStand)
        {
            _hitsBeforeStand = hitsBeforeStand;
            _hitsDone = 0;
        }

        public PlayerDecision Decide(PlayerDecisionContext context)
        {
            if (_hitsDone < _hitsBeforeStand)
            {
                _hitsDone++;
                return PlayerDecision.Hit;
            }

            return PlayerDecision.Stand;
        }
    }

    internal sealed class AlwaysDoubleDownIfPossibleStrategy : IPlayerStrategy
    {
        public PlayerDecision Decide(PlayerDecisionContext context)
        {
            if (context.CanDoubleDown)
            {
                return PlayerDecision.DoubleDown;
            }

            return PlayerDecision.Stand;
        }
    }

    internal sealed class SequenceStrategy : IPlayerStrategy
    {
        private readonly Queue<PlayerDecision> _decisions;

        public SequenceStrategy(params PlayerDecision[] decisions)
        {
            _decisions = new Queue<PlayerDecision>(decisions);
        }

        public PlayerDecision Decide(PlayerDecisionContext context)
        {
            if (_decisions.Count == 0)
            {
                return PlayerDecision.Stand;
            }

            return _decisions.Dequeue();
        }
    }
}
