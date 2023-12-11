using System.Text;

namespace aoc2023.day11;

class Solution
{
    static void Main(string[] args)
    {
        var galaxies = new Galaxies(File.ReadAllLines("inputs/day11.txt"));
        var result1 = galaxies.Expand()
            .PairwiseManhattanDistances()
            .Sum();

        Console.WriteLine(result1);
    }
}

class Galaxies
{
    private List<Galaxy> galaxies = [];
    public Galaxies(string[] lines)
    {
        int number = 0;
        for (int y = 0; y < lines.Length; y++)
            for (int x = 0; x < lines[y].Length; x++)
                if (lines[y][x] == '#') galaxies.Add(new(number++, new(x, y)));
    }

    public Galaxies Expand()
    {
        galaxies = galaxies
        .Select(g =>
        {
            var rowsWithGalaxies = galaxies.Where(g2 => g2.Coordinate.Y < g.Coordinate.Y).DistinctBy(g2 => g2.Coordinate.Y).Count();
            var colsWithGalaxies = galaxies.Where(g2 => g2.Coordinate.X < g.Coordinate.X).DistinctBy(g2 => g2.Coordinate.X).Count();
            var newX = g.Coordinate.X - colsWithGalaxies + g.Coordinate.X;
            var newY = g.Coordinate.Y - rowsWithGalaxies + g.Coordinate.Y;
            return new Galaxy(g.Number, new(newX, newY));
        }).ToList();
        return this;
    }

    internal IEnumerable<long> PairwiseManhattanDistances()
    {
        for (int i = 0; i < galaxies.Count; i++)
            for (int j = i + 1; j < galaxies.Count; j++)
                yield return Math.Abs(galaxies[i].Coordinate.X - galaxies[j].Coordinate.X) + Math.Abs(galaxies[i].Coordinate.Y - galaxies[j].Coordinate.Y);
    }

    public override string ToString()
    {
        var maxX = galaxies.Max(g => g.Coordinate.X);
        var maxY = galaxies.Max(g => g.Coordinate.Y);
        var sb = new StringBuilder();
        for (int y = 0; y <= maxY; y++)
        {
            for (int x = 0; x <= maxX; x++)
            {
                sb.Append(galaxies.Any(g => g.Coordinate == new Coordinate(x, y)) ? '#' : '.');
            }
            sb.AppendLine();

        }
        return sb.ToString();
    }
}

record Galaxy(int Number, Coordinate Coordinate);