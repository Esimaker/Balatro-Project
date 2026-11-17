using BalatroConsole.Models;

namespace BalatroConsole.Tests;

// Small helpers so a test hand reads as one line instead of five "new Card(...)" lines.
public static class TestHands
{
    public static List<ICard> Of(params (Rank rank, Suit suit)[] cards) =>
        cards.Select(c => (ICard)new Card(c.rank, c.suit)).ToList();

    // Reads a hand from text: "KS KD 3H 7C 9D". Rank: 2-9, T (ten), J, Q, K, A. Suit: H, D, C, S.
    public static List<ICard> Parse(string text) =>
        text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ParseCard).ToList();

    private static ICard ParseCard(string code)
    {
        Rank rank = code[0] switch
        {
            'T' => Rank.Ten,
            'J' => Rank.Jack,
            'Q' => Rank.Queen,
            'K' => Rank.King,
            'A' => Rank.Ace,
            >= '2' and <= '9' => (Rank)(code[0] - '0'),
            _ => throw new ArgumentException($"Unknown rank in '{code}'")
        };
        Suit suit = code[1] switch
        {
            'H' => Suit.Hearts,
            'D' => Suit.Diamonds,
            'C' => Suit.Clubs,
            'S' => Suit.Spades,
            _ => throw new ArgumentException($"Unknown suit in '{code}'")
        };
        return new Card(rank, suit);
    }
}
