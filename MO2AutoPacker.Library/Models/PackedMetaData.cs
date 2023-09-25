using System.Globalization;
using System.Text;

namespace MO2AutoPacker.Library.Models;

// TODO: Further define packed mod info, eg.
//       ModName    // Contains entire mod.
//       MyMod**    // Contains beginning of partial mod
//       **MyMod**  // Contains mid-point of partial mod.
//       **MyMod    // Contains end of partial mod.
public class PackedMetaData
{
    public const string ArchiveHeader = "archive=";
    public const string SizeHeader = "size=";
    public const string TimeHeader = "time=";

    public PackedMetaData(string archiveName, int size, DateTime packedAt, IEnumerable<string> mods)
    {
        ArchiveName = archiveName;
        Size = size;
        PackedAt = packedAt;
        Mods = mods.ToArray();
    }

    public string ArchiveName { get; }
    public int Size { get; }
    public DateTime PackedAt { get; }
    public string[] Mods { get; }

    public static PackedMetaData ReadFromString(string content)
    {
        using StringReader reader = new(content);

        string archive = ReadHeader(ArchiveHeader, reader.ReadLine());

        int size;
        if (!int.TryParse(ReadHeader(SizeHeader, reader.ReadLine()), out size))
            throw new FormatException($"Expected Int32 value for header '{SizeHeader}'");

        DateTime time;
        if (!DateTime.TryParse(ReadHeader(TimeHeader, reader.ReadLine()), DateTimeFormatInfo.InvariantInfo, out time))
            throw new FormatException($"Expected DateTime value for header '{TimeHeader}'");

        List<string> mods = new();
        string? nextMod = reader.ReadLine();
        while (nextMod != null)
        {
            nextMod = nextMod.Trim();
            if (nextMod != string.Empty)
                mods.Add(nextMod);

            nextMod = reader.ReadLine();
        }

        if (mods.Count == 0)
            throw new FormatException("Missing packed mod names");

        return new PackedMetaData(archive, size, time, mods);
    }

    private static string ReadHeader(string headerName, string? line)
    {
        if (line == null || !line.Contains(headerName))
            throw new FormatException($"Missing header '{headerName}'");

        string[] split = line.Trim()
            .Split(headerName, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (split.Length == 0)
            throw new FormatException($"Missing value for header '{headerName}'");

        return split[0].Trim();
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append(ArchiveHeader).AppendLine(ArchiveName);
        sb.Append(SizeHeader).AppendLine(Size.ToString());
        sb.Append(TimeHeader).AppendLine(PackedAt.ToString(DateTimeFormatInfo.InvariantInfo));
        foreach (string mod in Mods)
            sb.AppendLine(mod);

        return sb.ToString();
    }
}