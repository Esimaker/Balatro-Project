namespace BalatroConsole.Logic.Combinations;

// The one place that knows which combinations the game has.
// HandEvaluator only receives a list of IHandCombination, so it never mentions a concrete class.
// Adding a combination means: write the class, add one line here.
public static class HandCombinations
{
    public static List<IHandCombination> CreateAll() => new()
    {
        new HighCard(),
        new Pair(),
        new TwoPair(),
        new ThreeOfAKind(),
        new FourOfAKind()
    };
}
