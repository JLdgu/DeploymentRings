namespace DeploymentRings.Test;

public class HeaderValidationTests
{

    [Test]
    [Arguments(["A","B"])]
    [Arguments([ "A", "B", "C", "D", "E", "OS version", "G", "H", "I", "J", "K", "L" ])]
    [Arguments([ "A", "B", "C", "D", "E", "F", "OS version", "H", "I", "J", "K", "L" ])]
    public async Task Azuze_AD_Device_ID__or_OS_Version_not_found_throws(string[] header)
    {
        await Assert.That(() => InventorySummary.ValidateHeader(header))
            .Throws<InvalidDataException>();
    }

    [Test]
    [Arguments(4,5, new string[] {"A", "B", "C", "D", "Azure AD Device ID", "OS version", "G", "H", "I", "J", "K", "L" })]
    [Arguments(11, 6, new string[] { "A", "B", "C", "D", "E", "F", "OS version", "H", "I", "J", "K", "Azure AD Device ID" })]
    public async Task Valid_Headers_return_column_numbers( int azureIdColumn, int osVersionColumn,string[] header)
    {

        var actualColumns = InventorySummary.ValidateHeader(header);

        await Assert.That(actualColumns.Item1).IsEqualTo(azureIdColumn);
        await Assert.That(actualColumns.Item2).IsEqualTo(osVersionColumn);
    }
}