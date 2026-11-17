using BalatroConsole.Models;

namespace BalatroConsole.Logic.Combinations;

// Exactly three cards of one rank plus exactly two cards of another rank, e.g. K K K 4 4.
// It implements the interface itself: it needs two different groups, not one like OfAKindCombination.
public class FullHouse : IHandCombination
{
    public HandType Type => HandType.FullHouse;
    public string Name => "Full House";
    public int BasePoints => 50;
    public int BaseMultiplier => 4;

    public bool Matches(IReadOnlyList<ICard> cards) =>
        FindGroup(cards, 3) != null && FindGroup(cards, 2) != null;

    // All five cards: the three of a kind and the pair. Empty if it is not a full house.
    public IReadOnlyList<ICard> GetScoringCards(IReadOnlyList<ICard> cards)
    {
        IGrouping<Rank, ICard>? three = FindGroup(cards, 3);
        IGrouping<Rank, ICard>? pair = FindGroup(cards, 2);
        if (three == null || pair == null)
            return new List<ICard>();

        return three.Concat(pair).ToList();
    }

    // The highest rank that has exactly 'count' cards.
    private static IGrouping<Rank, ICard>? FindGroup(IReadOnlyList<ICard> cards, int count) =>
        cards.GroupBy(card => card.Rank)
             .Where(group => group.Count() == count)
             .OrderByDescending(group => group.Key)
             .FirstOrDefault();
}
