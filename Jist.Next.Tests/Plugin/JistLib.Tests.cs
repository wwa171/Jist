
using Xunit;

public class JistLibTests
{
    [Fact]
    public void RandomInclusiveShouldReturnARandomNumber()
    {
        var rnd = Jist.Next.Plugin.Lib.Jist.randomInclusive(1, 10);
        Assert.True(rnd > 0 && rnd <= 10);
    }

    [Fact]
    public void RandomShouldReturnARandomNumber()
    {
        var rnd = Jist.Next.Plugin.Lib.Jist.random(1, 10);
        Assert.True(rnd > 0 && rnd < 10);
    }
}
