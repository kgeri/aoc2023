
using System.Text.RegularExpressions;

namespace aoc2023.day2;

class Solution
{

    static void Main(string[] args)
    {
        var games = File.ReadAllLines("inputs/day2.txt")
        .Select(Game.Parse)
        .ToList();

        Selection limit = new(new Dictionary<string, int> { ["red"] = 12, ["green"] = 13, ["blue"] = 14 });
        var result1 = (from g in games
                       where g.Cubes.All(c => c.Below(limit))
                       select g.ID).Sum();
        Console.WriteLine(result1);

        var result2 = games
        .Select(g => g.FewestPossible())
        .Select(s => s.Power)
        .Sum();
        Console.WriteLine(result2);
    }
}

record Game(int ID, List<Selection> Cubes)
{
    internal static Game Parse(string line)
    {
        var groups = new Regex(@"Game (\d+): (.*)").Match(line).Groups;
        int id = int.Parse(groups[1].Value);
        var cubes = groups[2].Value.Split("; ")
        .Select(Selection.Parse)
        .ToList();
        return new(id, cubes);
    }

    public Selection FewestPossible()
    {
        var colorToMaxCount = Cubes.SelectMany(c => c.ColorToCount.Keys).Distinct()
        .ToDictionary(c => c, c => Cubes.Max(s => s.ColorToCount.GetValueOrDefault(c, 0)));
        return new(colorToMaxCount);
    }

    public override string ToString()
    {
        return $"Game {ID}: {String.Join("; ", Cubes)}";
    }
}

record Selection(Dictionary<string, int> ColorToCount)
{
    internal static Selection Parse(string line)
    {
        var colorCount = new Dictionary<string, int>();
        foreach (var en in line.Split(", ").Select(x => x.Split(" ")))
        {
            colorCount[en[1]] = int.Parse(en[0]);
        }
        return new(colorCount);
    }

    public int Power => ColorToCount.Values.Aggregate(1, (a, b) => a * b);

    public bool Below(Selection limit)
    {
        return ColorToCount.All(en => en.Value <= limit.ColorToCount[en.Key]);
    }

    public override string ToString()
    {
        return String.Join(", ", ColorToCount.Select(x => $"{x.Value} {x.Key}"));
    }
}