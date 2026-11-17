using BalatroConsole.Models;

namespace BalatroConsole.Logic.Combinations;

// Every poker hand (high card, pair, ...) implements this.
// The evaluator only talks to this interface, so adding a new combination
// means adding one new class without changing the evaluator's logic.
public interface IHandCombination
{
    HandType Type { get; }
    string Name { get; }

    // Base points and multiplier this combination gives before the cards are added.
    int BasePoints { get; }
    int BaseMultiplier { get; }

    // True if the played cards contain this combination.
    bool Matches(IReadOnlyList<ICard> cards);
}
