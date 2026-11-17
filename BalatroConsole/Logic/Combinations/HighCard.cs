using BalatroConsole.Models;

namespace BalatroConsole.Logic.Combinations;

public class HighCard : IHandCombination
{
    public HandType Type => HandType.HighCard;
    public string Name => "High Card";
    public int BasePoints => 5;
    public int BaseMultiplier => 1;

    // Any play with at least one card counts as a high card. It is the fallback.
    public bool Matches(IReadOnlyList<ICard> cards) => cards.Count > 0;

    // Only the single highest card scores. On a tie the first one played is used.
    public IReadOnlyList<ICard> GetScoringCards(IReadOnlyList<ICard> cards)
    {
        if (cards.Count == 0)
            return new List<ICard>();

        ICard highest = cards.MaxBy(card => card.Rank)!;
        return new List<ICard> { highest };
    }
}
