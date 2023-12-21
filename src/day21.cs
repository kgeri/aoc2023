

namespace aoc2023.day21;

class Solution
{
    static void Main(string[] args)
    {
        var garden = new Garden(File.ReadAllLines("inputs/day21.txt"));

        // Part 1
        var result1 = garden.NumberOfPlotsAfter(64);
        Console.WriteLine(result1);

        // Part 2
        var result2 = garden.NumberOfPlotsAfter(26501365);
        Console.WriteLine(result2);
    }
}

class Garden
{
    private readonly char[,] grid;

    internal Garden(string[] lines) { grid = lines.ToGrid(); }

    public long NumberOfPlotsAfter(int steps)
    {
        int width = Width();
        if (steps > width * 4)
        {
            // Note: I could have never come up with the below on my own, kudos to https://www.reddit.com/user/ProfONeill
            // But at least my code is faster :P
            int remainder = steps % width;
            long s1 = NumberOfPlotsAfter(remainder);
            long s2 = NumberOfPlotsAfter(remainder + width);
            long s3 = NumberOfPlotsAfter(remainder + 2 * width);

            // Especially this bit... WTF? Although from other answers I suspect it's the formula for https://en.wikipedia.org/wiki/Vandermonde_matrix for 3 points
            double a = (s1 - 2.0 * s2 + s3) / 2.0;
            double b = (-3.0 * s1 + 4.0 * s2 - s3) / 2.0;
            double c = s1;
            long n = steps / width;
            Console.WriteLine($"Equation: {a} n^2 + {b} n + {c}, with n = {n}");
            return (long)(a * n * n + b * n + c);
        }

        Direction[] dirs = Enum.GetValues<Direction>();
        Coordinate start = grid.Iterate2D().Where(c => grid.ValueAt(c) == 'S').First();
        HashSet<(Coordinate, Coordinate)> wavefront = [(start, new(0, 0))];
        for (int i = 0; i < steps; i++)
        {
            HashSet<(Coordinate, Coordinate)> newWF = new(wavefront.Count * 4);
            foreach (var (c, page) in wavefront)
            {
                foreach (var dir in dirs)
                {
                    var n = c.Neighbor(dir);
                    var nwrapped = grid.Wrapped(n);
                    var v = grid.ValueAt(nwrapped);
                    if (v == '#') continue;

                    if (!grid.ContainsCoordinate(n)) newWF.Add((nwrapped, page.Neighbor(dir)));
                    else newWF.Add((nwrapped, page));
                }
            }
            wavefront = newWF;
            // Debug :)
            // Console.WriteLine($"Step {i} => {wavefront.Count()}");
        }
        Console.WriteLine($"{steps} => {wavefront.Count}");
        return wavefront.Count;
    }

    internal int Width() => grid.GetLength(1);
}