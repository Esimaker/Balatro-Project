namespace BalatroConsole.Logic.Combinations;

// All the shared logic lives in OfAKindCombination, so this class only needs its own values.
public class FourOfAKind : OfAKindCombination
{
    public override HandType Type => HandType.FourOfAKind;
    public override string Name => "Four of a Kind";
    public override int BasePoints => 60;
    public override int BaseMultiplier => 7;
    protected override int RequiredCount => 4;
}
