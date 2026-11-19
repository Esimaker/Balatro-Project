using BalatroConsole.Models;

namespace BalatroConsole.Tests;

public class ScoreTests
{
    [Fact]
    public void Total_IsPointsTimesMultiplier()
    {
        Assert.Equal(60, new Score(30, 2).Total);
    }

    [Fact]
    public void AddPoints_ReturnsANewScore_AndLeavesTheOldOneAlone()
    {
        Score original = new(10, 2);

        Score changed = original.AddPoints(5);

        Assert.Equal(new Score(15, 2), changed);
        Assert.Equal(new Score(10, 2), original);   // immutable
    }

    [Fact]
    public void MultiplyBy_ReturnsANewScore_AndLeavesThePointsAlone()
    {
        Score original = new(10, 2);

        Score changed = original.MultiplyBy(2);

        Assert.Equal(new Score(10, 4), changed);
        Assert.Equal(new Score(10, 2), original);
    }

    [Fact]
    public void TwoScoresWithTheSameValues_AreEqual()
    {
        Assert.Equal(new Score(5, 1), new Score(5, 1));
        Assert.NotEqual(new Score(5, 1), new Score(5, 2));
    }
}
