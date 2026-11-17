using BalatroConsole.Models;

namespace BalatroConsole.Logic.Combinations;

public class Pair : IHandCombination
{
    public HandType Type => HandType.Pair;
    public string Name => "Pair";
    public int BasePoints => 10;
    public int BaseMultiplier => 2;

    // At least two cards share the same rank.
    public bool Matches(IReadOnlyList<ICard> cards) =>
        cards.GroupBy(card => card.Rank).Any(group => group.Count() >= 2);
}
