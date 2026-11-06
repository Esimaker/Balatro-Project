using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class DeckTests
{
    [Fact]
    public void NewDeck_Has52UniqueCards()
    {
        // Arrange
        Deck deck = new();

        // Act
        int uniqueCount = deck.Cards.Select(c => (c.Rank, c.Suit)).Distinct().Count();

        // Assert
        Assert.Equal(52, deck.Cards.Count);
        Assert.Equal(52, uniqueCount);
    }

    [Fact]
    public void Draw5_LeavesHandWith5AndDeckWith47()
    {
        Deck deck = new();

        List<ICard> hand = deck.Draw(5);

        Assert.Equal(5, hand.Count);
        Assert.Equal(47, deck.Cards.Count);
        Assert.Equal(5, deck.DrawnCards.Count);
    }

    [Fact]
    public void Shuffle_ChangesOrder_ButKeepsSameCards()
    {
        // A fixed seed makes this test deterministic instead of "probably passes".
        Deck deck = new(new Random(42));
        List<ICard> before = deck.Cards.ToList();

        deck.Shuffle();

        Assert.NotEqual(before, deck.Cards);
        Assert.Equal(before.ToHashSet(), deck.Cards.ToHashSet());
    }
}
