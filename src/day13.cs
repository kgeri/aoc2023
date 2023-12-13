using System.Text;

namespace aoc2023.day13;

class Solution
{
    static void Main(string[] args)
    {
        var result1 = File.ReadAllText("inputs/day13.txt").Split("\n\n")
            .Select(LavaIsland.Parse)
            .Select(li => li.GetMirrorColumn() ?? li.GetMirrorRow() * 100)
            .Sum();
        Console.WriteLine(result1);

        var result2 = File.ReadAllText("inputs/day13.txt").Split("\n\n")
            .Select(LavaIsland.Parse)
            .Select(li => li.FindMirrorRowColumnWithSmudge())
            .Sum();
        Console.WriteLine(result2);
    }
}

class LavaIsland
{
    private readonly BitSet[] rows;
    private readonly BitSet[] cols;

    private LavaIsland(string[] lines)
    {
        int nr = lines.Length;
        int nc = lines[0].Length;

        rows = new BitSet[nr];
        for (int i = 0; i < rows.Length; i++) rows[i] = new BitSet();

        cols = new BitSet[nc];
        for (int i = 0; i < cols.Length; i++) cols[i] = new BitSet();

        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < cols.Length; x++)
            {
                this[y, x] = lines[y][x] == '#';
            }
        }
    }

    public bool this[int y, int x]
    {
        get { return rows[y][x]; }
        set
        {
            rows[y][x] = value;
            cols[x][y] = value;
        }
    }

    public bool this[int seq]
    {
        get
        {
            int y = seq / cols.Length;
            int x = seq % cols.Length;
            return this[y, x];
        }
        set
        {
            int y = seq / cols.Length;
            int x = seq % cols.Length;
            this[y, x] = value;
        }
    }

    internal static LavaIsland Parse(string lines) => new(lines.Split("\n", StringSplitOptions.RemoveEmptyEntries));

    public int FindMirrorRowColumnWithSmudge()
    {
        var or = GetMirrorRow();
        var oc = GetMirrorColumn();
        int len = rows.Length * cols.Length;
        for (int i = 0; i < len; i++)
        {
            if (i > 0) this[i - 1] = !this[i - 1];
            this[i] = !this[i];

            var value = GetMirrorColumn(oc) ?? GetMirrorRow(or) * 100;
            if (value != null) return (int)value;
        }

        throw new Exception("Not found in:\n" + ToString());
    }

    public int? GetMirrorColumn(int? except = null)
    {
        bool isMirror(int x)
        {
            for (int i = 0; x - i - 1 >= 0 && x + i < cols.Length; i++)
                if (cols[x - i - 1] != cols[x + i]) return false;
            return true;
        }
        for (int x = 1; x < cols.Length; x++)
            if (x != except && isMirror(x)) return x;
        return null;
    }

    public int? GetMirrorRow(int? except = null)
    {
        bool isMirror(int y)
        {
            for (int i = 0; y - i - 1 >= 0 && y + i < rows.Length; i++)
                if (rows[y - i - 1] != rows[y + i]) return false;
            return true;
        }
        for (int y = 1; y < rows.Length; y++)
            if (y != except && isMirror(y)) return y;
        return null;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < cols.Length; x++) sb.Append(this[y, x] ? '#' : '.');
            sb.AppendLine();
        }
        return sb.ToString();
    }
}