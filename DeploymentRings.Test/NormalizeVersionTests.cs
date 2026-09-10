namespace DeploymentRings.Test;

public class NormalizeVersionTests
{
    [Test]
    [Arguments("26.6", "26.6.0")]
    [Arguments("26.6.1", "26.6.1")]
    [Arguments("1.0", "1.0.0")]
    [Arguments("10.22.7", "10.22.7")]
    public async Task Normalizes_Expected_Versions(
        string input,
        string expected)
    {
        await Assert.That(
            InventorySummary.NormalizeVersion(input))
            .IsEqualTo(expected);
    }

    [Test]
    [Arguments("")]
    [Arguments(" ")]
    [Arguments(null)]
    public async Task Empty_Version_Returns_Default(
        string? input)
    {
        await Assert.That(
            InventorySummary.NormalizeVersion(input!))
            .IsEqualTo("0.0.0");
    }
}
