namespace BalatroConsole.Logic.Combinations;

// Ordered from weakest to strongest. HandEvaluator sorts on this order,
// so a new combination must be put at the right place in this list.
public enum HandType
{
    HighCard,
    Pair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind
}
