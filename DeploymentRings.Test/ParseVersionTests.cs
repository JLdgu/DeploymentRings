namespace DeploymentRings.Test;

public class ParseVersionTests
{
    [Test]
    public async Task Version_26_6_Equals_26_6_0()
    {
        var v1 = InventorySummary.ParseVersion("26.6");
        var v2 = InventorySummary.ParseVersion("26.6.0");

        await Assert.That(v1).IsEqualTo(v2);
    }

    [Test]
    public async Task Versions_Sort_Correctly()
    {
        var versions = new[]
        {
            "26.6.1",
            "26.5",
            "26.6"
        };

        var sorted = versions
            .OrderBy(InventorySummary.ParseVersion)
            .ToArray();

        await Assert.That(sorted)
            .IsEquivalentTo(new[]
            {
                "26.5",
                "26.6",
                "26.6.1"
            });
    }
}