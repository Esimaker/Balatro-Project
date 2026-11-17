namespace BalatroConsole.Logic.Combinations;

public class ThreeOfAKind : OfAKindCombination
{
    public override HandType Type => HandType.ThreeOfAKind;
    public override string Name => "Three of a Kind";
    public override int BasePoints => 30;
    public override int BaseMultiplier => 3;
    protected override int RequiredCount => 3;
}
