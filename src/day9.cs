using System.Security.Cryptography;

namespace aoc2023.day9;

class Solution
{
    static void Main(string[] args)
    {
        var result1 = File.ReadAllLines("inputs/day9.txt")
            .Select(ValueHistory.Parse)
            .Select(history => new Estimator(history))
            .Select(estimator => estimator.Estimate())
            .Sum();

        Console.WriteLine(result1);
    }
}

record ValueHistory(long[] Values)
{
    internal static ValueHistory Parse(string line) => new(line.Split(" ").Select(long.Parse).ToArray());
}

class Estimator
{
    private readonly List<long> lastValues = [];

    internal Estimator(ValueHistory history)
    {
        long[] diffs = history.Values;
        while (diffs.Any(d => d != 0))
        {
            lastValues.Add(diffs.Last());
            diffs = Diff(diffs);
        }
    }

    public long Estimate() => lastValues.Sum();

    private static long[] Diff(long[] values) => values.Zip(values.Skip(1), (a1, a2) => a2 - a1).ToArray();

    public override string ToString() => $"Estimator: {string.Join(", ", lastValues)}";
}