namespace aoc2023.day3;

using System.Text.RegularExpressions;
using aoc2023;

class Solution
{
    static void Main(string[] args)
    {
        var schematic = new Schematic(File.ReadAllText("inputs/day3.txt"));
        var result1 = schematic.ValidParts()
        .Select(p => p.Value)
        .Sum();

        var result2 = schematic.GearPartPairs()
        .Select(gp => gp.Item1.Value * gp.Item2.Value)
        .Sum();

        Console.WriteLine(result2);
    }
}

class Schematic
{
    private readonly char[][] grid;
    private readonly List<Part> parts = [];

    internal Schematic(string input)
    {
        var lines = input.Split("\n");
        grid = lines.Select(r => r.ToCharArray()).ToArray()!;

        Regex number = new(@"\d+");
        for (int y = 0; y < lines.Length; y++)
        {
            foreach (Match m in number.Matches(lines[y]))
            {
                parts.Add(new Part(new(m.Index, y), new(m.Index + m.Length - 1, y), int.Parse(m.Value)));
            }
        }
    }

    public IEnumerable<Part> ValidParts()
    {
        return parts.FindAll(IsValidPart);
    }

    internal IEnumerable<(Part, Part)> GearPartPairs()
    {
        foreach (var gc in GearCoordinates())
        {
            var parts = grid.NeighborsAndDiagonals(gc).SelectMany(GetPartsAt).Distinct();
            if (parts.Count() == 2)
            {
                yield return (parts.First(), parts.Last());
            }
        }
    }

    private bool IsValidPart(Part p)
    {
        return p.Coordinates().Any(c => grid.NeighborsAndDiagonals(c).Any(nc => IsSymbol(grid.ValueAt(nc))));
    }

    private static bool IsSymbol(char value) => value != '.' && value != default && !char.IsNumber(value);

    public IEnumerable<Coordinate> GearCoordinates() => grid.Iterate2D().Where(c => grid.ValueAt(c) == '*');

    private IEnumerable<Part> GetPartsAt(Coordinate c) => parts.Where(p => p.Coordinates().Contains(c)).Distinct();

    public override string ToString()
    {
        return String.Join("\n", grid.Select(r => new String(r)));
    }
}

record Part(Coordinate Start, Coordinate End, int Value)
{

    public IEnumerable<Coordinate> Coordinates()
    {
        for (int x = Start.X; x <= End.X; x++)
        {
            yield return new(x, Start.Y);
        }
    }
}