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

        var result2 = File.ReadAllLines("inputs/day12.txt")
            .Select(line => new Springs(line, 5))
            .Select(springs => springs.PossibleArrangements())
            .Sum();
        Console.WriteLine(result2);
    }
}

class Springs
{
    private readonly string springs;
    private readonly int[] groups;

    internal Springs(string line, int multiplier = 1)
    {
        var parts = line.Split(' ');
        springs = parts[0];
        springs = string.Join("?", Enumerable.Repeat(springs, multiplier)) + ".";
        groups = Enumerable.Repeat(parts[1].Split(',').Select(int.Parse), multiplier).SelectMany(x => x).Append(springs.Length + 1).ToArray();
    }

    internal long PossibleArrangements()
    {
        // Disclaimer: I get the concept of memoization and vaguely understand how this works, but no clue about the edge cases...
        var f = new long[springs.Length + 1, groups.Length + 1, springs.Length + 2];
        f[0, 0, 0] = 1;
        for (int i = 0; i < springs.Length; i++)
        {
            for (int j = 0; j < groups.Length; j++)
            {
                for (int k = 0; k < springs.Length + 1; k++)
                {
                    long cur = f[i, j, k];
                    if (cur == 0) continue;
                    if (springs[i] == '.' || springs[i] == '?') // Current spring is operational or unknown
                        if (k == 0 || k == groups[j - 1]) // ... and there is no current group or current group is complete
                            f[i + 1, j, 0] += cur; // ...then we proceed with the current group
                    if (springs[i] == '#' || springs[i] == '?') // Current spring is broken or unknown
                        if (k == 0) // ... and there is no current group
                            f[i + 1, j + 1, 1] += cur; // ...then we start a new group
                        else
                            f[i + 1, j, k + 1] += cur; // ...otherwise we continue with the current group
                }
            }
        }
        return f[springs.Length, groups.Length - 1, 0];
    }

    public override string ToString() => $"springs={springs}, damagePattern={string.Join(',', groups)}";
}
