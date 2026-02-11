using Blackjack.Core.Domain;

namespace Blackjack.Core.Abstractions;

// Abstraktion så vi kan mocke/fake deck i tests.
public interface IDeck
{
    int Count { get; }
    Card Draw();
}
