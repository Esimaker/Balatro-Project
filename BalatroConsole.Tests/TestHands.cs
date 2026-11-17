using BalatroConsole.Models;

namespace BalatroConsole.Tests;

// Small helper so a test hand reads as one line instead of five "new Card(...)" lines.
public static class TestHands
{
    public static List<ICard> Of(params (Rank rank, Suit suit)[] cards) =>
        cards.Select(c => (ICard)new Card(c.rank, c.suit)).ToList();
}
