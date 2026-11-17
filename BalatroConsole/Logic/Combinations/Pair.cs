namespace BalatroConsole.Logic.Combinations;

public class Pair : OfAKindCombination
{
    public override HandType Type => HandType.Pair;
    public override string Name => "Pair";
    public override int BasePoints => 10;
    public override int BaseMultiplier => 2;
    protected override int RequiredCount => 2;
}
