using BalatroConsole.Logic.Combinations;
using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class FullHouseTests
{
    private readonly FullHouse _fullHouse = new();

    [Fact]
    public void ThreeKingsAndTwoFours_Matches_AndAllFiveScore()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.King, Suit.Spades), (Rank.King, Suit.Hearts), (Rank.King, Suit.Clubs),
            (Rank.Four, Suit.Spades), (Rank.Four, Suit.Hearts));

        Assert.True(_fullHouse.Matches(cards));
        Assert.Equal(5, _fullHouse.GetScoringCards(cards).Count);
    }

    [Fact]
    public void ThreeOfAKindWithTwoDifferentCards_DoesNotMatch()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.King, Suit.Spades), (Rank.King, Suit.Hearts), (Rank.King, Suit.Clubs),
            (Rank.Four, Suit.Spades), (Rank.Five, Suit.Hearts));

        Assert.False(_fullHouse.Matches(cards));
        Assert.Empty(_fullHouse.GetScoringCards(cards));
    }

    [Fact]
    public void TwoPairs_DoesNotMatch()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.King, Suit.Spades), (Rank.King, Suit.Hearts), (Rank.Four, Suit.Clubs),
            (Rank.Four, Suit.Spades), (Rank.Five, Suit.Hearts));

        Assert.False(_fullHouse.Matches(cards));
    }

    [Fact]
    public void FourOfAKindPlusOneCard_DoesNotMatch()
    {
        // Needs exactly 3 + exactly 2. Four of a kind has its own combination.
        List<ICard> cards = TestHands.Of(
            (Rank.King, Suit.Spades), (Rank.King, Suit.Hearts), (Rank.King, Suit.Clubs),
            (Rank.King, Suit.Diamonds), (Rank.Four, Suit.Hearts));

        Assert.False(_fullHouse.Matches(cards));
    }

    [Fact]
    public void OnlyThreeCardsOfOneRank_DoesNotMatch()
    {
        List<ICard> cards = TestHands.Of(
            (Rank.King, Suit.Spades), (Rank.King, Suit.Hearts), (Rank.King, Suit.Clubs));

        Assert.False(_fullHouse.Matches(cards));
    }

    [Fact]
    public void NoCards_DoesNotMatch()
    {
        Assert.False(_fullHouse.Matches(new List<ICard>()));
        Assert.Empty(_fullHouse.GetScoringCards(new List<ICard>()));
    }
}
