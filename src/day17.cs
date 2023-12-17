
namespace aoc2023.day17;

class Solution
{
    static void Main(string[] args)
    {
        var result1 = new HeatLossAStar(File.ReadAllLines("inputs/day17.txt"))
            .MinHeatLoss();
        Console.WriteLine(result1);
    }
}

record State(Coordinate Position, Direction LastDir, int SameDirSteps);

class HeatLossAStar : AStar<State>
{
    private readonly int[,] grid;

    internal HeatLossAStar(string[] lines)
    {
        grid = new int[lines.Length, lines[0].Length];
        for (int y = 0; y < grid.GetLength(0); y++)
            for (int x = 0; x < grid.GetLength(1); x++)
                grid[y, x] = (int)char.GetNumericValue(lines[y][x]);
    }

    protected override double Distance(State current, State neighbor) =>
        grid.ContainsCoordinate(neighbor.Position) ? grid.ValueAt(neighbor.Position) : double.PositiveInfinity;

    protected override double Heuristic(State current) => grid.GetLength(0) - current.Position.Y + grid.GetLength(1) - current.Position.X;

    protected override IEnumerable<State> Neighbors(State c)
    {
        if (c.SameDirSteps < 3)
            yield return new(c.Position.Neighbor(c.LastDir), c.LastDir, c.SameDirSteps + 1);

        Direction left = c.LastDir.Left();
        yield return new(c.Position.Neighbor(left), left, 1);

        Direction right = c.LastDir.Right();
        yield return new(c.Position.Neighbor(right), right, 1);
    }

    public int MinHeatLoss()
    {
        var lastPos = grid.LastCoordinate();
        var path = FindShortestPath(new(new(0, 0), Direction.East, 0), s => s.Position == lastPos);
        // Debug :)
        foreach (var (a, b) in path.Zip(path.Skip(1)))
            Console.WriteLine($"({a.Position.X},{a.Position.Y}) -{a.Position.DirectionTo(b.Position)}-> ({b.Position.X},{b.Position.Y}), heat={grid.ValueAt(b.Position)}");
        return path
            .Skip(1) // Heat loss ignored on the start coordinate
            .Select(c => grid.ValueAt(c.Position))
            .Sum();
    }

    public override string ToString() => grid.GridToString();
}
