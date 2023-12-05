
namespace aoc2023.day5;

class Solution
{
    static void Main(string[] args)
    {
        var sections = File.ReadAllText("inputs/day5.txt").Split("\n\n");
        var seeds = sections[0].Replace("seeds: ", "").Split(" ").Select(long.Parse).ToList();
        var mappings = sections.Skip(1).Select(Mapping.Parse).ToList();

        long result1 = long.MaxValue;
        foreach (var seed in seeds)
        {
            var location = seed;
            foreach (var mapping in mappings) location = mapping.MapToDest(location);
            result1 = Math.Min(result1, location);
        }

        Console.WriteLine(result1);
    }
}

record Mapping(string Name, List<MapFunction> Mappers)
{
    private static readonly MapFunction DefaultMapFunction = new(0, 0, long.MaxValue);

    public static Mapping Parse(string input)
    {
        var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var name = lines[0].Split(" map:")[0];
        var mappers = lines.Skip(1).Select(MapFunction.Parse).Append(DefaultMapFunction).ToList();
        return new(name, mappers);
    }

    public long MapToDest(long source) => Mappers.First(mapper => mapper.IsInRange(source)).MapToDest(source);
}

class MapFunction(long destRangeStart, long sourceRangeStart, long length)
{
    internal static MapFunction Parse(string line)
    {
        var parts = line.Split(" ").Select(long.Parse).ToList();
        return new(parts[0], parts[1], parts[2]);
    }

    public bool IsInRange(long source) => source >= sourceRangeStart && source < sourceRangeStart + length;

    public long MapToDest(long source) => destRangeStart + (source - sourceRangeStart);
}
