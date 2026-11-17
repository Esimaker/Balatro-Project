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
}
