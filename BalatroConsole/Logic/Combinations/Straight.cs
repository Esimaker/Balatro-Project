using BalatroConsole.Models;

namespace BalatroConsole.Logic.Combinations;

// Five cards with consecutive ranks, e.g. 5 6 7 8 9. The suits do not matter.
// The ace counts high (10 J Q K A) and low (A 2 3 4 5), but there is no wrap-around (Q K A 2 3).
// It implements the interface itself: "a run of ranks" does not fit OfAKindCombination.
public class Straight : IHandCombination
{
    private const int RunLength = 5;
    private const int LowAceValue = 1;

    public HandType Type => HandType.Straight;
    public string Name => "Straight";
    public int BasePoints => 35;
    public int BaseMultiplier => 4;

    public bool Matches(IReadOnlyList<ICard> cards) => FindRun(cards).Count == RunLength;

    // The five cards of the run. The other played cards do not score. Empty if there is no straight.
    public IReadOnlyList<ICard> GetScoringCards(IReadOnlyList<ICard> cards) => FindRun(cards);

    private static List<ICard> FindRun(IReadOnlyList<ICard> cards)
    {
        // One card per rank value. An ace is stored twice: as 14 (high) and as 1 (low).
        Dictionary<int, ICard> cardByValue = new();
        foreach (ICard card in cards)
        {
            cardByValue.TryAdd((int)card.Rank, card);
            if (card.Rank == Rank.Ace)
                cardByValue.TryAdd(LowAceValue, card);
        }

        // Try the highest run first, so with more than 5 cards the best straight is used.
        for (int highest = (int)Rank.Ace; highest >= LowAceValue + RunLength - 1; highest--)
        {
            List<int> wanted = Enumerable.Range(highest - RunLength + 1, RunLength).ToList();
            if (wanted.All(cardByValue.ContainsKey))
                return wanted.Select(value => cardByValue[value]).ToList();
        }

        return new List<ICard>();
    }
}
