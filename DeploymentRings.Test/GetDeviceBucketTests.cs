namespace DeploymentRings.Test;

public class GetDeviceBucketTests
{
    [Test]
    [Arguments('0', "Ring 1")]
    [Arguments('1', "Ring 1")]
    [Arguments('2', "Ring 1")]
    [Arguments('3', "Ring 2")]
    [Arguments('5', "Ring 2")]
    [Arguments('6', "Ring 3")]
    [Arguments('8', "Ring 3")]
    [Arguments('9', "Ring 4")]
    [Arguments('A', "Ring 4")]
    [Arguments('B', "Ring 4")]
    [Arguments('C', "Ring 5")]
    [Arguments('F', "Ring 5")]
    [Arguments('a', "Ring 4")]
    [Arguments('f', "Ring 5")]
    public async Task Maps_To_Expected_Bucket(
        char input,
        string expected)
    {
        await Assert.That(
            InventorySummary.GetDeviceDeploymentRing(input))
            .IsEqualTo(expected);
    }

    [Test]
    [Arguments('G')]
    [Arguments('X')]
    [Arguments('-')]
    public async Task Invalid_Characters_Return_Unknown(char input)
    {
        await Assert.That(
            InventorySummary.GetDeviceDeploymentRing(input))
            .IsEqualTo("Unknown");
    }
}
