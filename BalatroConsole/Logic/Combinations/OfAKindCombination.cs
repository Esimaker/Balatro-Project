using BalatroConsole.Models;

namespace BalatroConsole.Logic.Combinations;

// Shared code for combinations of "N cards with the same rank" (pair, three of a kind, ...).
// A child class only has to say how many cards it needs and what it is worth.
public abstract class OfAKindCombination : IHandCombination
{
    public abstract HandType Type { get; }
    public abstract string Name { get; }
    public abstract int BasePoints { get; }
    public abstract int BaseMultiplier { get; }

    // How many cards of the same rank are needed (2 for a pair, 3 for three of a kind).
    protected abstract int RequiredCount { get; }

    public bool Matches(IReadOnlyList<ICard> cards) => FindBestGroup(cards) != null;

    public IReadOnlyList<ICard> GetScoringCards(IReadOnlyList<ICard> cards)
    {
        IGrouping<Rank, ICard>? group = FindBestGroup(cards);
        if (group == null)
            return new List<ICard>();

        return group.ToList();
    }

    // Exactly RequiredCount cards of one rank: 7-7-7 is NOT a pair, it is three of a kind.
    // If several ranks qualify (e.g. K K 3 3), the highest rank wins so the result is predictable.
    private IGrouping<Rank, ICard>? FindBestGroup(IReadOnlyList<ICard> cards) =>
        cards.GroupBy(card => card.Rank)
             .Where(group => group.Count() == RequiredCount)
             .OrderByDescending(group => group.Key)
             .FirstOrDefault();
}
