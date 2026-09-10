namespace DeploymentRings.Test;

public class HeaderValidationTests
{
    [Test]
    public void Valid_Headers_Do_Not_Throw()
    {
        var headers = new[]
        {
            "A",
            "B",
            "C",
            "D",
            "Azure AD Device ID",
            "OS version"
        };

        InventorySummary.ValidateHeaders(headers);
    }

    [Test]
    public async Task Invalid_Device_Id_Header_Throws()
    {
        var headers = new[]
        {
            "A",
            "B",
            "C",
            "D",
            "Wrong Header",
            "OS version"
        };

        await Assert.That(() =>
            InventorySummary.ValidateHeaders(headers))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task Invalid_OS_Version_Header_Throws()
    {
        var headers = new[]
        {
            "A",
            "B",
            "C",
            "D",
            "Azure AD Device ID",
            "OS Version"
        };

        await Assert.That(() =>
            InventorySummary.ValidateHeaders(headers))
            .Throws<InvalidDataException>();
    }
}