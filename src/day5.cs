using System.Text;

namespace aoc2023.day5;

class Solution
{
    static void Main(string[] args)
    {
        var sections = File.ReadAllText("inputs/day5.txt").Split("\n\n");
        var seedValues = sections[0].Replace("seeds: ", "").Split(" ")
            .Select(long.Parse).ToList();

        // Part1
        // var seeds = seedValues.Select(s => new Range(s, s));

        // Part 2
        var seeds = seedValues.ZipWithNext()
            .Select(item => new Range(item.a, item.a + item.b - 1));

        var mappings = sections.Skip(1).Select(Mapping.Parse).ToList();

        IEnumerable<Range> ranges = seeds;
        foreach (var mapping in mappings)
        {
            ranges = ranges.SelectMany(mapping.Transform);
        }

        var result1 = ranges.Min(s => s.Start);

        Console.WriteLine(result1);
    }
}

record Range(long Start, long End)
{
    internal Range? Intersect(Range range)
    {
        if (Start > range.End || End < range.Start) return null;
        var start = Math.Max(Start, range.Start);
        var end = Math.Min(End, range.End);
        return new Range(start, end);
    }
}

record Mapping(string Name, List<Shift> Shifts)
{
    public static Mapping Parse(string input)
    {
        var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        var name = lines[0].Split(" map:")[0];
        var shifts = lines.Skip(1)
            .Select(Shift.Parse)
            .OrderBy(s => s.Source.Start)
            .ToList();
        var firstShiftStart = shifts.First().Source.Start;
        var lastShiftEnd = shifts.Last().Source.End;
        shifts = shifts
            .Prepend(new Shift(new(0, firstShiftStart - 1), 0))
            .Append(new Shift(new(lastShiftEnd + 1, long.MaxValue), 0))
            .ToList();
        return new(name, shifts);
    }

    public IEnumerable<Range> Transform(Range range)
    {
        foreach (var shift in Shifts)
        {
            var intersection = shift.Source.Intersect(range);
            if (intersection == null) continue;
            yield return new Range(intersection.Start + shift.Value, intersection.End + shift.Value);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Name} map:");
        foreach (var shift in Shifts)
        {
            sb.AppendLine($"{shift.Source.Start} -> {shift.Source.End} + {shift.Value}");
        }
        return sb.ToString();
    }
}

record Shift(Range Source, long Value)
{
    internal static Shift Parse(string line)
    {
        var parts = line.Split(" ").Select(long.Parse).ToList();
        var destStart = parts[0];
        var sourceStart = parts[1];
        var length = parts[2];
        return new(new(sourceStart, sourceStart + length - 1), destStart - sourceStart);
    }
}
