using System.Text;
using System.Text.RegularExpressions;

namespace aoc2023.day12;

class Solution
{
    static void Main(string[] args)
    {
        var result1 = File.ReadAllLines("inputs/day12.txt")
            .Select(line => new Springs(line))
            .Select(springs => springs.PossibleArrangements())
            .Sum();
        Console.WriteLine(result1);
    }
}

class Springs
{
    private readonly string springs;
    private readonly Regex damagePattern;

    internal Springs(string line)
    {
        var parts = line.Split(' ');
        springs = parts[0].Replace('.', ' '); // Easier to regex this one :)

        var groups = parts[1].Split(',').Select(int.Parse).ToArray();
        var damages = new StringBuilder("^ *?"); // Can start with any number of good springs (`.`)
        for (int i = 0; i < groups.Length; i++)
        {
            if (i > 0) damages.Append(" +"); // Having at least one good spring in between
            for (int j = 0; j < groups[i]; j++)
                damages.Append('#'); // Followed by a number of bad springs (`#
        }
        damagePattern = new(damages.Append(" *$").ToString()); // Can end with any number of good springs
    }

    internal int PossibleArrangements()
    {
        int[] unknownIndices = springs.Select((c, i) => (c, i)).Where(t => t.c == '?').Select(t => t.i).ToArray();
        int maxValue = 1 << unknownIndices.Length;
        char[] s = springs.ToCharArray();
        int arrangements = 0;
        for (int i = 0; i < maxValue; i++)
        {
            for (int j = 0; j < unknownIndices.Length; j++)
            {
                s[unknownIndices[j]] = (i & (1 << j)) > 0 ? '#' : ' ';
            }
            if (damagePattern.IsMatch(new string(s))) arrangements++;
        }
        return arrangements;
    }

    public override string ToString() => $"springs={springs}, damagePattern={damagePattern}";
}
