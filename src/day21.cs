
namespace aoc2023.day21;

class Solution
{
    static void Main(string[] args)
    {
        var garden = new Garden(File.ReadAllLines("inputs/day21.txt"));

        var result1 = garden.NumberOfPlotsAfter(64);
        Console.WriteLine(result1);
    }
}

class Garden
{
    private readonly char[,] grid;

    internal Garden(string[] lines) { grid = lines.ToGrid(); }

    public int NumberOfPlotsAfter(int steps)
    {
        Coordinate start = grid.Iterate2D().Where(c => grid.ValueAt(c) == 'S').First();
        IEnumerable<Coordinate> wavefront = [start];
        for (int i = 0; i < steps; i++)
        {
            wavefront = wavefront
                .SelectMany(c => c.Neighbors()
                .Where(n =>
                {
                    var v = grid.ValueAt(n);
                    return v == '.' || v == 'S';
                }))
                .Distinct();
        }
        return wavefront.Count();
    }
}