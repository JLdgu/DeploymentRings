namespace DeploymentRings.Test;

public class SummariseTests
{
    [Test]
    public async Task Counts_Rows_By_Bucket_And_Version()
    {
        var rows = new[]
        {
            new DeviceInventoryRow("0AAA", "26.6"),
            new DeviceInventoryRow("1BBB", "26.6"),
            new DeviceInventoryRow("A111", "26.6"),
            new DeviceInventoryRow("B222", "26.6.1")
        };

        var result = InventorySummary.Summarise(rows);

        await Assert.That(result[("Ring 1", "26.6.0")])
            .IsEqualTo(2);

        await Assert.That(result[("Ring 4", "26.6.0")])
            .IsEqualTo(1);

        await Assert.That(result[("Ring 4", "26.6.1")])
            .IsEqualTo(1);
    }

    [Test]
    public async Task Unknown_Bucket_Is_Ignored()
    {
        var rows = new[]
        {
            new DeviceInventoryRow("X123", "26.6")
        };

        var result = InventorySummary.Summarise(rows);

        await Assert.That(result.Count)
            .IsEqualTo(0);
    }

    [Test]
    public async Task Matching_Rows_Are_Aggregated()
    {
        var rows = Enumerable.Range(0, 10)
            .Select(_ =>
                new DeviceInventoryRow(
                    "C123456",
                    "26.6"));

        var result = InventorySummary.Summarise(rows);

        await Assert.That(result[("Ring 5", "26.6.0")]).IsEqualTo(10);
    }
}
