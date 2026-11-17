using BalatroConsole.Models;

namespace BalatroConsole.Logic.Combinations;

// Two different ranks that each appear at least twice (e.g. K K 3 3).
// It does not fit OfAKindCombination (that one looks for ONE rank), so it implements the interface itself.
public class TwoPair : IHandCombination
{
    public HandType Type => HandType.TwoPair;
    public string Name => "Two Pair";
    public int BasePoints => 20;
    public int BaseMultiplier => 2;

    public bool Matches(IReadOnlyList<ICard> cards) => FindPairs(cards).Count == 2;

    // The cards of both pairs: 4 cards in total. Empty if there are not two pairs.
    public IReadOnlyList<ICard> GetScoringCards(IReadOnlyList<ICard> cards)
    {
        List<List<ICard>> pairs = FindPairs(cards);
        if (pairs.Count < 2)
            return new List<ICard>();

        return pairs.SelectMany(pair => pair).ToList();
    }

    // The two highest-ranking pairs. A pair is exactly 2 cards of a rank, so K K K 4 4 is not
    // "two pair" (that is a full house) and 6 6 6 6 is not two pair either.
    private static List<List<ICard>> FindPairs(IReadOnlyList<ICard> cards) =>
        cards.GroupBy(card => card.Rank)
             .Where(group => group.Count() == 2)
             .OrderByDescending(group => group.Key)
             .Take(2)
             .Select(group => group.ToList())
             .ToList();
}
