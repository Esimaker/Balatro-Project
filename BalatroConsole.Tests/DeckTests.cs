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
    public void Deal5_LeavesHandWith5AndDeckWith47()
    {
        Deck deck = new();

        List<ICard> hand = deck.Deal(5);

        Assert.Equal(5, hand.Count);
        Assert.Equal(47, deck.Cards.Count);
        Assert.Equal(5, deck.DrawnCards.Count);
    }

    [Fact]
    public void Deal8_Leaves44_AndDealtAndRemainingCardsDoNotOverlap()
    {
        Deck deck = new(new Random(1));
        deck.Shuffle();

        List<ICard> dealt = deck.Deal(8);

        Assert.Equal(8, dealt.Count);
        Assert.Equal(44, deck.Cards.Count);
        Assert.Empty(dealt.Intersect(deck.Cards));
        Assert.Equal(52, dealt.Concat(deck.Cards).Distinct().Count());   // nothing lost, nothing doubled
    }

    [Fact]
    public void Deal_TakesCardsFromTheTop_InOrder()
    {
        Deck deck = new();
        ICard first = deck.Cards[0];
        ICard second = deck.Cards[1];

        List<ICard> dealt = deck.Deal(2);

        Assert.Same(first, dealt[0]);
        Assert.Same(second, dealt[1]);
        Assert.DoesNotContain(first, deck.Cards);
    }

    [Fact]
    public void Deal_MoreThanRemaining_ThrowsAndLeavesTheDeckUntouched()
    {
        Deck deck = new();
        deck.Deal(50);

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => deck.Deal(3));

        Assert.Contains("only 2", ex.Message);
        Assert.Equal(2, deck.Cards.Count);
    }

    [Fact]
    public void Deal_ExactlyTheRemainingCards_EmptiesTheDeck()
    {
        Deck deck = new();

        deck.Deal(52);

        Assert.Empty(deck.Cards);
    }

    [Fact]
    public void Deal_ANegativeNumber_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Deck().Deal(-1));
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
