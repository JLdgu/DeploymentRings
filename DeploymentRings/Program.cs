using DeploymentRings;
using System.IO.Compression;
using System.Text;

string downloadsFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "Downloads");

FileInfo zipFile = new DirectoryInfo(downloadsFolder)
    .GetFiles("deviceswithinventory*.zip")
    .OrderByDescending(f => f.LastWriteTimeUtc)
    .FirstOrDefault()
    ?? throw new FileNotFoundException("No deviceswithinventory*.zip found in Downloads.");

Console.WriteLine($"Using ZIP: {zipFile.FullName}");

using ZipArchive archive = ZipFile.OpenRead(zipFile.FullName);

ZipArchiveEntry csvEntry = archive.Entries
    .FirstOrDefault(e =>
        e.Name.StartsWith("deviceswithinventory", StringComparison.OrdinalIgnoreCase) &&
        e.Name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
    ?? throw new FileNotFoundException("No deviceswithinventory*.csv found in ZIP.");

Console.WriteLine($"Using CSV: {csvEntry.Name}");

using Stream csvStream = csvEntry.Open();
using StreamReader reader = new(csvStream);

string? headerLine = reader.ReadLine();

if (string.IsNullOrWhiteSpace(headerLine))
{
    throw new InvalidDataException("CSV is empty.");
}

string[] headers = ParseCsvLine(headerLine);
InventorySummary.ValidateHeaders(headers);

var rows = new List<DeviceInventoryRow>();

string? line;
while ((line = reader.ReadLine()) != null)
{
    string[] columns = ParseCsvLine(line);

    if (columns.Length <= 5)
    {
        continue;
    }

    rows.Add(new DeviceInventoryRow(
        AzureAdDeviceId: columns[4].Trim(),
        OsVersion: columns[5].Trim()));
}

var counts = InventorySummary.Summarise(rows);

string outputPath = Path.Combine(
    downloadsFolder,
    $"DeviceInventorySummary_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

using StreamWriter writer = new(outputPath, false, Encoding.UTF8);

string[] rings =
[
    "Ring 1",
    "Ring 2",
    "Ring 3",
    "Ring 4",
    "Ring 5"
];

writer.WriteLine($"OSVersion,{string.Join(',', rings)}");

foreach (string version in counts.Keys
             .Select(key => key.Version)
             .Distinct()
             .OrderByDescending(InventorySummary.ParseVersion))
{
    string[] ringCounts = rings
        .Select(ring => counts.TryGetValue((ring, version), out int count)
            ? count.ToString()
            : "0")
        .ToArray();

    writer.WriteLine($"{version},{string.Join(',', ringCounts)}");
}

Console.WriteLine($"Output written to: {outputPath}");

static string[] ParseCsvLine(string line)
{
    List<string> fields = new();
    StringBuilder current = new();

    bool inQuotes = false;

    foreach (char c in line)
    {
        if (c == '"')
        {
            inQuotes = !inQuotes;
            continue;
        }

        if (c == ',' && !inQuotes)
        {
            fields.Add(current.ToString());
            current.Clear();
            continue;
        }

        current.Append(c);
    }

    fields.Add(current.ToString());

    return fields.ToArray();
}
