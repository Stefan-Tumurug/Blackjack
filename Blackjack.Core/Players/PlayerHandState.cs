namespace Blackjack.Core.Players
{
    // Keeps per-hand flags separate from Hand itself.
    // This will scale when we later introduce split (multiple hands per player).
    public sealed class PlayerHandState
    {
        public bool HasStood { get; private set; }
        public bool HasDoubledDown { get; private set; }

        public void Stand()
        {
            HasStood = true;
        }

        public void MarkDoubledDown()
        {
            HasDoubledDown = true;
        }

        public void Reset()
        {
            HasStood = false;
            HasDoubledDown = false;
        }
    }
}
