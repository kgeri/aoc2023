
namespace aoc2023.day23;

class Solution
{
    static void Main(string[] args)
    {
        var map = new ForestMap(File.ReadAllLines("inputs/day23.txt"));
        var result1 = map.LengthOfMostScenicRoute();
        Console.WriteLine(result1);
    }
}

class ForestMap
{
    private readonly char[,] grid;

    internal ForestMap(string[] lines) { grid = lines.ToGrid(); }

    public override string ToString() => grid.GridToString();

    public int LengthOfMostScenicRoute()
    {
        Coordinate start = new(1, 0);
        Coordinate target = new(grid.GetLength(1) - 2, grid.GetLength(0) - 1);

        int[,] maxDistances = new int[grid.GetLength(1), grid.GetLength(0)];

        Stack<(Coordinate prev, Coordinate current)> stack = [];
        stack.Push((start, start));
        while (stack.TryPop(out (Coordinate prev, Coordinate current) step))
        {
            var (prev, c) = step;
            maxDistances[c.Y, c.X] = Math.Max(maxDistances[c.Y, c.X], maxDistances[prev.Y, prev.X] + 1);

            foreach (var n in step.current.Neighbors())
            {
                char? v = grid.ValueAt(n);
                if (n == prev || v == default(char) || v == '#') continue;

                Direction dir = step.current.DirectionTo(n);
                if ((dir == Direction.North && v == 'v')
                || (dir == Direction.South && v == '^')
                || (dir == Direction.East && v == '<')
                || (dir == Direction.West && v == '>')) continue;

                stack.Push((c, n));
            }
        }

        return maxDistances[target.Y, target.X] - 1; // Stepping into the forest is not counted
    }
}