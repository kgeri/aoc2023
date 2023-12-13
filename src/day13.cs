
using System.Text;
using aoc2023.copilot;

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
    }
}

class LavaIsland(bool[,] pattern)
{
    private readonly bool[,] pattern = pattern;

    internal static LavaIsland Parse(string lines)
    {
        var ls = lines.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var pattern = new bool[ls.Length, ls[0].Length];
        for (int y = 0; y < pattern.GetLength(0); y++)
        {
            for (int x = 0; x < pattern.GetLength(1); x++)
                pattern[y, x] = ls[y][x] == '#';
        }
        return new(pattern);
    }

    public int? GetMirrorColumn()
    {
        bool isMirror(int x)
        {
            for (int i = 0; x - i - 1 >= 0 && x + i < pattern.GetLength(1); i++)
                if (!pattern.Column(x - i - 1).SequenceEqual(pattern.Column(x + i))) return false;
            return true;
        }
        for (int x = 1; x < pattern.GetLength(1); x++)
            if (isMirror(x)) return x;
        return null;
    }

    public int? GetMirrorRow()
    {
        bool isMirror(int y)
        {
            for (int i = 0; y - i - 1 >= 0 && y + i < pattern.GetLength(0); i++)
                if (!pattern.Row(y - i - 1).SequenceEqual(pattern.Row(y + i))) return false;
            return true;
        }
        for (int y = 1; y < pattern.GetLength(0); y++)
            if (isMirror(y)) return y;
        return null;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y < pattern.GetLength(0); y++)
        {
            for (int x = 0; x < pattern.GetLength(1); x++) sb.Append(pattern[y, x] ? '#' : '.');
            sb.AppendLine();
        }
        return sb.ToString();
    }
}