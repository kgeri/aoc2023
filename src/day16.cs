namespace aoc2023.day16;

class Solution
{
    static void Main(string[] args)
    {
        var grid = new LightGrid(File.ReadAllLines("inputs/day16.txt"));

        var result1 = grid.CalculateEnergizedTiles((Direction.West, new(0, 0)));
        Console.WriteLine(result1);

        var result2 = grid.CalculateMostEnergizedConfiguration();
        Console.WriteLine(result2);
    }
}

class LightGrid
{
    private readonly Tile[,] grid;

    internal LightGrid(string[] lines)
    {
        grid = new Tile[lines.Length, lines[0].Length];
        for (int y = 0; y < grid.GetLength(0); y++)
            for (int x = 0; x < grid.GetLength(1); x++)
                grid[y, x] = lines[y][x] switch
                {
                    '.' => new Empty(new(x, y)),
                    '/' => new RightMirror(new(x, y)),
                    '\\' => new LeftMirror(new(x, y)),
                    '|' => new UpDownSplitter(new(x, y)),
                    '-' => new LeftRightSplitter(new(x, y)),
                    char t => throw new ArgumentException($"Unknown tile: {t}")
                };
    }

    public int CalculateEnergizedTiles((Direction, Coordinate) startPos)
    {
        HashSet<(Direction, Coordinate)> seen = [];
        Stack<(Direction, Coordinate)> stack = new();
        stack.Push(startPos);

        while (stack.TryPop(out (Direction d, Coordinate c) pos))
        {
            Tile tile = grid.ValueAt(pos.c)!;
            if (tile is null) continue; // Out of bounds

            tile.energized = true;

            foreach (var nextPos in tile.Neighbors(pos.d))
                if (seen.Add(nextPos)) stack.Push(nextPos);
        }

        var result = grid.Iterate2D().Where(c => grid.ValueAt(c)!.energized).Count();
        foreach (var tile in grid) tile.energized = false; // Resetting the grid
        return result;
    }

    public int CalculateMostEnergizedConfiguration()
    {
        int result = 0;
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            result = Math.Max(result, CalculateEnergizedTiles((Direction.West, new(0, y))));
            result = Math.Max(result, CalculateEnergizedTiles((Direction.East, new(grid.GetLength(1) - 1, y))));
        }
        for (int x = 0; x < grid.GetLength(1); x++)
        {
            result = Math.Max(result, CalculateEnergizedTiles((Direction.North, new(x, 0))));
            result = Math.Max(result, CalculateEnergizedTiles((Direction.South, new(x, grid.GetLength(0) - 1))));
        }
        return result;
    }

    public override string ToString()
    {
        var overlay = new char[grid.GetLength(0), grid.GetLength(1)];
        for (int y = 0; y < overlay.GetLength(1); y++)
            for (int x = 0; x < overlay.GetLength(0); x++)
            {
                var tile = grid[y, x];
                if (tile.energized) overlay[y, x] = '#';
                else overlay[y, x] = tile.type;
            }
        return overlay.GridToString();
    }
}

abstract class Tile(Coordinate c, char type)
{
    public readonly char type = type;
    public bool energized = false;
    protected Coordinate c = c;
    public abstract IEnumerable<(Direction, Coordinate)> Neighbors(Direction d);
}

class Empty(Coordinate c) : Tile(c, '.')
{
    public override IEnumerable<(Direction, Coordinate)> Neighbors(Direction d)
    {
        switch (d)
        {
            case Direction.North: yield return (d, c.NeighborSouth()); break;
            case Direction.South: yield return (d, c.NeighborNorth()); break;
            case Direction.East: yield return (d, c.NeighborWest()); break;
            case Direction.West: yield return (d, c.NeighborEast()); break;
        }
    }
}

class RightMirror(Coordinate c) : Tile(c, '/')
{
    public override IEnumerable<(Direction, Coordinate)> Neighbors(Direction d)
    {
        switch (d)
        {
            case Direction.North: yield return (Direction.East, c.NeighborWest()); break;
            case Direction.South: yield return (Direction.West, c.NeighborEast()); break;
            case Direction.East: yield return (Direction.North, c.NeighborSouth()); break;
            case Direction.West: yield return (Direction.South, c.NeighborNorth()); break;
        }
    }
}

class LeftMirror(Coordinate c) : Tile(c, '\\')
{
    public override IEnumerable<(Direction, Coordinate)> Neighbors(Direction d)
    {
        switch (d)
        {
            case Direction.North: yield return (Direction.West, c.NeighborEast()); break;
            case Direction.South: yield return (Direction.East, c.NeighborWest()); break;
            case Direction.East: yield return (Direction.South, c.NeighborNorth()); break;
            case Direction.West: yield return (Direction.North, c.NeighborSouth()); break;
        }
    }
}

class UpDownSplitter(Coordinate c) : Tile(c, '|')
{
    public override IEnumerable<(Direction, Coordinate)> Neighbors(Direction d)
    {
        switch (d)
        {
            case Direction.North: yield return (d, c.NeighborSouth()); break;
            case Direction.South: yield return (d, c.NeighborNorth()); break;
            case Direction.East:
                yield return (Direction.South, c.NeighborNorth());
                yield return (Direction.North, c.NeighborSouth());
                break;
            case Direction.West:
                yield return (Direction.South, c.NeighborNorth());
                yield return (Direction.North, c.NeighborSouth());
                break;
        }
    }
}

class LeftRightSplitter(Coordinate c) : Tile(c, '-')
{
    public override IEnumerable<(Direction, Coordinate)> Neighbors(Direction d)
    {
        switch (d)
        {
            case Direction.North:
                yield return (Direction.West, c.NeighborEast());
                yield return (Direction.East, c.NeighborWest());
                break;
            case Direction.South:
                yield return (Direction.West, c.NeighborEast());
                yield return (Direction.East, c.NeighborWest());
                break;
            case Direction.East: yield return (d, c.NeighborWest()); break;
            case Direction.West: yield return (d, c.NeighborEast()); break;
        }
    }
}