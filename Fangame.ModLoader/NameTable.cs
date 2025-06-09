namespace Fangame.ModLoader;

public class NameTable
{
    private static NameTable? s_globalTable;
    private readonly Dictionary<string, string[]> _table;

    public static NameTable Global => s_globalTable ??= new NameTable(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GlobalNameTable.csv"));

    public NameTable(string path)
    {
        _table = [];
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] names = lines[i].Split(',');
            _table.Add(names[0], names[1..]);
        }
    }

    public bool CheckIn(string nameToCheck, params Span<string> rows)
    {
        foreach (var name in rows)
        {
            if (_table.TryGetValue(name, out string[]? row))
            {
                if (row.Contains(nameToCheck))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
