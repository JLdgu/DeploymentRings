namespace DeploymentRings;

public static class InventorySummary
{
    public static void ValidateHeaders(string[] headers)
    {
        if (headers.Length <= 5)
        {
            throw new InvalidDataException("CSV does not contain expected columns.");
        }

        if (!string.Equals(headers[4], "Azure AD Device ID", StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                $"Expected column 4 to be 'Azure AD Device ID' but found '{headers[4]}'.");
        }

        if (!string.Equals(headers[5], "OS version", StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                $"Expected column 5 to be 'OS version' but found '{headers[5]}'.");
        }
    }

    public static string GetDeviceBucket(char c)
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

    public static int BucketSortOrder(string bucket) =>
        bucket switch
        {
            "Ring 1" => 0,
            "Ring 2" => 1,
            "Ring 3" => 2,
            "Ring 4" => 3,
            "Ring 5" => 4,
            _ => 999
        };

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

    public static IReadOnlyDictionary<(string Bucket, string Version), int>
        Summarise(IEnumerable<DeviceInventoryRow> rows)
    {
        var counts = new Dictionary<(string Bucket, string Version), int>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.AzureAdDeviceId))
            {
                continue;
            }

            string bucket = GetDeviceBucket(row.AzureAdDeviceId[0]);

            if (bucket == "Unknown")
            {
                continue;
            }

            string version = NormalizeVersion(row.OsVersion);

            var key = (bucket, version);

            counts.TryGetValue(key, out int currentCount);

            counts[key] = currentCount + 1;
        }

        return counts;
    }
}

public record DeviceInventoryRow(
    string AzureAdDeviceId,
    string OsVersion);