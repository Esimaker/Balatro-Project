using BalatroConsole.Models;

namespace BalatroConsole.Logic.Combinations;

// Five cards of the same suit, in any order. The ranks do not matter.
// A straight flush (5 6 7 8 9 of hearts) is also a flush. Flush is stronger than Straight,
// so the evaluator scores it as a flush.
public class Flush : IHandCombination
{
    private const int RequiredCount = 5;

    public HandType Type => HandType.Flush;
    public string Name => "Flush";
    public int BasePoints => 40;
    public int BaseMultiplier => 4;

    public bool Matches(IReadOnlyList<ICard> cards) => FindSuitGroup(cards) != null;

    // The five highest cards of the suit. The other played cards do not score.
    public IReadOnlyList<ICard> GetScoringCards(IReadOnlyList<ICard> cards)
    {
        IGrouping<Suit, ICard>? group = FindSuitGroup(cards);
        if (group == null)
            return new List<ICard>();

        return group.OrderByDescending(card => card.Rank).Take(RequiredCount).ToList();
    }

    // If more than one suit qualifies (only possible with 10+ cards), the suit with the highest card wins.
    private static IGrouping<Suit, ICard>? FindSuitGroup(IReadOnlyList<ICard> cards) =>
        cards.GroupBy(card => card.Suit)
             .Where(group => group.Count() >= RequiredCount)
             .OrderByDescending(group => group.Max(card => card.Rank))
             .FirstOrDefault();
}
