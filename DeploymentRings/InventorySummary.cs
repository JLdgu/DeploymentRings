namespace DeploymentRings;

public static class InventorySummary
{
    public static Tuple<int, int> ValidateHeader(string[] headers)
    {
        if (headers.Length != 12 && headers.Length != 53)
        {
            throw new InvalidDataException("CSV does not contain expected columns.");
        }

        int AzureAdDeviceIdColumn = Array.IndexOf(headers, "Azure AD Device ID");
        if (AzureAdDeviceIdColumn == -1)
        {
            throw new InvalidDataException("Unable to find column 'Azure AD Device ID'.");
        }
        int OsVersionColumn = Array.IndexOf(headers, "OS version");
        if (OsVersionColumn == -1)
        {
            throw new InvalidDataException("Unable to find column 'OS version'.");
        }

        return Tuple.Create(AzureAdDeviceIdColumn, OsVersionColumn);
    }

    public static string GetDeviceDeploymentRing(char c)
    {
        c = char.ToUpperInvariant(c);

        return c switch
        {
            '0' or '1' or '2' => "Ring 1",
            '3' or '4' or '5' => "Ring 2",
            '6' or '7' or '8' => "Ring 3",
            '9' or 'A' or 'B' => "Ring 4",
            'C' or 'D' or 'E' or 'F' => "Ring 5",
            _ => "Unknown"
        };
    }


    public static string NormalizeVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return "0.0.0";
        }

        string[] parts = version.Split('.', StringSplitOptions.RemoveEmptyEntries);

        return parts.Length switch
        {
            2 => $"{parts[0]}.{parts[1]}.0",
            3 => $"{parts[0]}.{parts[1]}.{parts[2]}",
            _ => "0.0.0"
        };
    }

    public static Version ParseVersion(string version)
    {
        string normalized = NormalizeVersion(version);

        return Version.TryParse(normalized, out Version? result)
            ? result
            : new Version(0, 0, 0);
    }

    public static IReadOnlyDictionary<(string Bucket, string Version), int> Summarise(IEnumerable<DeviceInventoryRow> rows)
    {
        var counts = new Dictionary<(string Bucket, string Version), int>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.AzureAdDeviceId))
            {
                continue;
            }

            string deploymentRing = GetDeviceDeploymentRing(row.AzureAdDeviceId[0]);

            if (deploymentRing == "Unknown")
            {
                continue;
            }

            string version = NormalizeVersion(row.OsVersion);

            var key = (deploymentRing, version);

            counts.TryGetValue(key, out int currentCount);

            counts[key] = currentCount + 1;
        }

        return counts;
    }
}

public record DeviceInventoryRow(
    string AzureAdDeviceId,
    string OsVersion);