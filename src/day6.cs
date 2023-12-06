namespace aoc2023.day6;

class Solution
{
    static void Main(string[] args)
    {
        var lines = File.ReadAllLines("inputs/day6.txt");
        var times = lines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse).ToArray();
        var distances = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse).ToArray();
        var timeDistancePairs = times.Zip(distances, (time, distance) => (time, distance));

        var result1 = timeDistancePairs.Select(Solution.NumberOfWays).Aggregate((a, b) => a * b);

        Console.WriteLine(result1);
    }

    private static int NumberOfWays((int time, int distance) race)
    {
        int ways = 0;
        for (int charge = 1; charge < race.time; charge++)
        {
            if (charge * (race.time - charge) > race.distance) ways++;
        }
        return ways;
    }
}
