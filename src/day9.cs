using System.Security.Cryptography;

namespace aoc2023.day9;

class Solution
{
    static void Main(string[] args)
    {
        var estimators = File.ReadAllLines("inputs/day9.txt")
            .Select(ValueHistory.Parse)
            .Select(history => new Estimator(history));

        // Part 1
        var result1 = estimators
        .Select(estimator => estimator.EstimateNext())
        .Sum();
        Console.WriteLine(result1);

        // Part 2
        var result2 = estimators
        .Select(estimator => estimator.EstimatePrevious())
        .Sum();
        Console.WriteLine(result2);
    }
}

record ValueHistory(long[] Values)
{
    internal static ValueHistory Parse(string line) => new(line.Split(" ").Select(long.Parse).ToArray());
}

class Estimator
{
    private readonly List<long> firstValues = [];
    private readonly List<long> lastValues = [];

    internal Estimator(ValueHistory history)
    {
        long[] diffs = history.Values;
        while (diffs.Any(d => d != 0))
        {
            firstValues.Add(diffs.First());
            lastValues.Add(diffs.Last());
            diffs = Diff(diffs);
        }
    }

    public long EstimateNext() => lastValues.Sum();

    public long EstimatePrevious()
    {
        long result = firstValues.Last();
        for (int i = firstValues.Count - 2; i >= 0; i--)
        {
            result = firstValues[i] - result;
        }
        return result;
    }

    private static long[] Diff(long[] values) => values.Zip(values.Skip(1), (a1, a2) => a2 - a1).ToArray();

    public override string ToString() => $"Estimator: firstValues={string.Join(", ", firstValues)}, lastValues={string.Join(", ", lastValues)}";
}