namespace aoc2023.day23;

class Solution
{
    static void Main(string[] args)
    {
        var map = new ForestMap(File.ReadAllLines("inputs/day23.txt"));
        var result1 = map.LengthOfMostScenicRoute();
        Console.WriteLine(result1);

        var result2 = map.LengthOfMostScenicRouteIgnoringSlopes();
        Console.WriteLine(result2);
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

        int maxDistance = 0;

        Stack<(HashSet<Coordinate> prev, Coordinate current)> stack = [];
        stack.Push(([], start));
        while (stack.TryPop(out (HashSet<Coordinate> prev, Coordinate current) step))
        {
            var (prev, c) = step;
            if (c == target) maxDistance = Math.Max(maxDistance, prev.Count + 1);

            foreach (var n in c.Neighbors())
            {
                char? v = grid.ValueAt(n);
                if (prev.Contains(n) || v == default(char) || v == '#') continue;

                Direction dir = c.DirectionTo(n);
                if ((dir == Direction.North && v == 'v')
                || (dir == Direction.South && v == '^')
                || (dir == Direction.East && v == '<')
                || (dir == Direction.West && v == '>')) continue;

                stack.Push((new(prev) { n }, n));
            }
        }

        return maxDistance - 1; // Stepping into the forest is not counted
    }

    public int LengthOfMostScenicRouteIgnoringSlopes()
    {
        Dictionary<Coordinate, List<(Coordinate to, int steps)>> neighbors =
                Simplify(grid)
                .GroupBy(e => e.A)
                .ToDictionary(g => g.Key,
                    g => g.Select(e => (e.B, e.Steps)).ToList()
                );
        List<(Coordinate to, int steps)> neighborsOf(Coordinate c) => neighbors[c];
        Console.WriteLine($"Simplified graph to {neighbors.Count} vertices");

        Coordinate start = new(1, 0);
        Coordinate target = new(grid.GetLength(1) - 2, grid.GetLength(0) - 1);

        int maxDistance = 0;

        Stack<(HashSet<Coordinate> seen, Coordinate current, int steps)> stack = [];
        stack.Push(([], start, 0));
        while (stack.TryPop(out (HashSet<Coordinate>, Coordinate, int) step))
        {
            var (seen, c, steps) = step;
            if (c == target) maxDistance = Math.Max(maxDistance, steps);

            foreach (var (n, nsteps) in neighborsOf(c))
            {
                if (seen.Contains(n)) continue;
                stack.Push((new(seen) { n }, n, steps + nsteps));
            }
        }

        return maxDistance;
    }

    private static List<Edge> Simplify(char[,] grid)
    {
        Coordinate start = new(1, 0);
        Coordinate target = new(grid.GetLength(1) - 2, grid.GetLength(0) - 1);

        HashSet<Coordinate> seen = [];
        IEnumerable<Edge> walkAndCollectEdges(Coordinate from, Coordinate current, int distance)
        {
            seen.Add(current);
            var neighbors = current.Neighbors()
                .Where(n => !seen.Contains(n) && grid.ContainsCoordinate(n) && grid.ValueAt(n) != '#')
                .ToList();
            if (neighbors.Count == 0 && current == target)
            {
                yield return new(current, from, distance);
                yield return new(from, current, distance);
            }
            else if (neighbors.Count == 1)
            {
                var next = neighbors.First();
                foreach (var edge in walkAndCollectEdges(from, next, distance + 1))
                    yield return edge;
            }
            else
            {
                yield return new(current, from, distance);
                yield return new(from, current, distance);
                foreach (var n in neighbors)
                    foreach (var edge in walkAndCollectEdges(current, n, 1))
                        yield return edge;
            }
        }

        return walkAndCollectEdges(start, start, 0).Distinct().ToList();
    }
}

record Edge(Coordinate A, Coordinate B, int Steps);
