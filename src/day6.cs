namespace aoc2023.day6;

class Solution
{
    static void Main(string[] args)
    {
        var lines = File.ReadAllLines("inputs/day6.txt");
        var times = lines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse).ToArray();
        var distances = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(long.Parse).ToArray();
        var timeDistancePairs = times.Zip(distances, (time, distance) => (time, distance));

        // Part 1
        var result1 = timeDistancePairs.Select(Solution.NumberOfWays).Aggregate((a, b) => a * b);
        Console.WriteLine(result1);

        // Part 2
        var time = long.Parse(lines[0].Replace("Time:", "").Replace(" ", ""));
        var distance = long.Parse(lines[1].Replace("Distance:", "").Replace(" ", ""));

        var result2 = Solution.NumberOfWays((time, distance));
        Console.WriteLine(result2);
    }

    private static long NumberOfWays((long time, long distance) race)
    {
        long ways = 0;
        for (long charge = 1; charge < race.time; charge++)
        {
            if (charge * (race.time - charge) > race.distance) ways++;
        }
        return ways;
    }
}
