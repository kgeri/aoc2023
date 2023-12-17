
namespace aoc2023.day17;

class Solution
{
    static void Main(string[] args)
    {
        // Note: this ran for quite a while... should probably tweak the heuristics
        var result = new HeatLossAStar(File.ReadAllLines("inputs/day17.txt"))
            .MinHeatLoss();
        Console.WriteLine(result);
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
        // if (c.SameDirSteps < 3) // Part 1
        if (c.SameDirSteps < 10) // Part 2
            yield return new(c.Position.Neighbor(c.LastDir), c.LastDir, c.SameDirSteps + 1);

        Direction left = c.LastDir.Left();
        Direction right = c.LastDir.Right();

        if (c.Position == new Coordinate(0, 0) || c.SameDirSteps >= 4) // Part 2
        {
            yield return new(c.Position.Neighbor(left), left, 1);
            yield return new(c.Position.Neighbor(right), right, 1);
        }
    }

    public int MinHeatLoss()
    {
        var lastPos = grid.LastCoordinate();
        // var path = FindShortestPath(new(new(0, 0), Direction.East, 0), s => s.Position == lastPos); // Part 1
        var path = FindShortestPath(new(new(0, 0), Direction.East, 0), s => s.Position == lastPos && s.SameDirSteps >= 4); // Part 2
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
