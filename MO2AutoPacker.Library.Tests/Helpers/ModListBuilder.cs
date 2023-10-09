using System.Text;
using MO2AutoPacker.Library.Models;

namespace MO2AutoPacker.Library.Tests.Helpers;

internal class ModListBuilder
{
    private const string ModListFileName = "modlist.txt";

    private readonly StringBuilder _builder = new();

    private readonly Queue<IModListItem> _modListItems = new();

    private static string RandomString => Guid.NewGuid().ToString().Replace('-', ' ');

    public IEnumerable<IModListItem> ModListItems => _modListItems;

    public ModListBuilder AppendComment() => AppendComment(RandomString);

    public ModListBuilder AppendComment(string commentText)
    {
        _builder.Append("# ").AppendLine(commentText);
        return this;
    }

    public ModListBuilder AppendSeparator() => AppendSeparator(RandomString);

    public ModListBuilder AppendSeparator(string name)
    {
        _builder.Append('-').Append(name).AppendLine("_separator");
        _modListItems.Enqueue(new ModSeparator(name));
        return this;
    }

    public ModListBuilder AddMod(bool isEnabled) => AddMod(RandomString, isEnabled);

    public ModListBuilder AddMod(string name, bool isEnabled)
    {
        char prefix = isEnabled ? '+' : '-';
        _builder.Append(prefix).AppendLine(name);
        _modListItems.Enqueue(new Mod(name, isEnabled));
        return this;
    }

    public ModListBuilder AppendDLC()
    {
        _builder.AppendLine("*DLC: DLCBattlehornCastle")
            .AppendLine("*DLC: DLCFrostcrag")
            .AppendLine("*DLC: DLCHorseArmor")
            .AppendLine("*DLC: DLCMehrunesRazor")
            .AppendLine("*DLC: DLCOrrery")
            .AppendLine("*DLC: DLCShiveringIsles")
            .AppendLine("*DLC: DLCSpellTomes")
            .AppendLine("*DLC: DLCThievesDen")
            .AppendLine("*DLC: DLCVileLair")
            .AppendLine("*DLC: Knights");

        return this;
    }

    public DirectoryInfo WriteFile(DirectoryInfo directory) => WriteFile(directory, out _);

    public DirectoryInfo WriteFile(DirectoryInfo directory, out FileInfo file)
    {
        string path = Path.Combine(directory.FullName, ModListFileName);
        File.WriteAllText(path, ToString());
        file = new FileInfo(path);
        return directory;
    }

    public void CreateModDirectories(DirectoryInfo modsDir)
    {
        foreach (IModListItem mod in _modListItems.Where(li => li is Mod))
            Directory.CreateDirectory(Path.Combine(modsDir.FullName, mod.Name));
    }

    public override string ToString() => _builder.ToString();

    public static ModListBuilder BuildRandom()
    {
        ModListBuilder instance = new();

        for (var i = 0; i < 100; i++)
        {
            int rnd = Random.Shared.Next(100);
            switch (rnd)
            {
                case < 3:
                    instance.AppendComment();
                    break;
                case < 12:
                    instance.AppendSeparator();
                    break;
                case < 30:
                    instance.AddMod(false);
                    break;
                default:
                    instance.AddMod(true);
                    break;
            }
        }

        instance.AppendDLC();
        return instance;
    }
}