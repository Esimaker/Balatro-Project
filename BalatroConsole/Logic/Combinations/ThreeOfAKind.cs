using BalatroConsole.Models;

namespace BalatroConsole.Logic.Combinations;

public class ThreeOfAKind : IHandCombination
{
    public HandType Type => HandType.ThreeOfAKind;
    public string Name => "Three of a Kind";
    public int BasePoints => 30;
    public int BaseMultiplier => 3;

    // At least three cards share the same rank.
    public bool Matches(IReadOnlyList<ICard> cards) =>
        cards.GroupBy(card => card.Rank).Any(group => group.Count() >= 3);
}
