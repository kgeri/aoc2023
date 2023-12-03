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

        Console.WriteLine(result1);
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

    private bool IsValidPart(Part p)
    {
        return p.Coordinates().Any(c => grid.NeighborsAndDiagonals(c).Any(nc => IsSymbol(grid.ValueAt(nc))));
    }

    private static bool IsSymbol(char value) => value != '.' && value != default && !char.IsNumber(value);

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