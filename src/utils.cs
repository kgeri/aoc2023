using System.Text;

namespace aoc2023;

public enum Direction
{
    North, South, East, West
}

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction d) => d switch
    {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        Direction.West => Direction.East,
        _ => throw new NotImplementedException(),
    };

    public static Direction Left(this Direction d) => d switch
    {
        Direction.North => Direction.West,
        Direction.South => Direction.East,
        Direction.East => Direction.North,
        Direction.West => Direction.South,
        _ => throw new NotImplementedException(),
    };

    public static Direction Right(this Direction d) => d switch
    {
        Direction.North => Direction.East,
        Direction.South => Direction.West,
        Direction.East => Direction.South,
        Direction.West => Direction.North,
        _ => throw new NotImplementedException(),
    };
}

public record Coordinate(long X, long Y)
{
    public Coordinate Neighbor(Direction d, long distance = 1) => d switch
    {
        Direction.North => new(X, Y - distance),
        Direction.South => new(X, Y + distance),
        Direction.East => new(X + distance, Y),
        Direction.West => new(X - distance, Y),
        _ => throw new NotImplementedException(),
    };

    public IEnumerable<Coordinate> Neighbors()
    {
        yield return NeighborWest();
        yield return NeighborNorth();
        yield return NeighborEast();
        yield return NeighborSouth();
    }

    public Coordinate NeighborWest() => new(X - 1, Y);
    public Coordinate NeighborNorthWest() => new(X - 1, Y - 1);
    public Coordinate NeighborNorth() => new(X, Y - 1);
    public Coordinate NeighborNorthEast() => new(X + 1, Y - 1);
    public Coordinate NeighborEast() => new(X + 1, Y);
    public Coordinate NeighborSouthEast() => new(X + 1, Y + 1);
    public Coordinate NeighborSouth() => new(X, Y + 1);
    public Coordinate NeighborSouthWest() => new(X - 1, Y + 1);

    public bool IsInside(List<Coordinate> polygon)
    {
        bool isInside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; i++)
        {
            var ci = polygon[i];
            var cj = polygon[j];
            bool intersects = cj.Y > Y ^ ci.Y > Y && X < (ci.X - cj.X) * (Y - cj.Y) / (ci.Y - cj.Y) + cj.X;
            isInside ^= intersects;
            j = i;
        }
        return isInside;
    }

    public Direction DirectionTo(Coordinate target)
    {
        if (X == target.X) return Y < target.Y ? Direction.South : Direction.North;
        if (Y == target.Y) return X < target.X ? Direction.East : Direction.West;
        throw new ArgumentException($"Can't determine direction {this} -> {target}");
    }

    public override string ToString() => $"({X},{Y})";
}

public record Coordinate3D(long X, long Y, long Z)
{
    public Coordinate3D Translate(long dx = 0, long dy = 0, long dz = 0) => new(X + dx, Y + dy, Z + dz);

    public override string ToString() => $"({X},{Y},{Z})";
}

public record Range(long Start, long End)
{
    public static readonly Range EMPTY = new(0, -1);

    public Range? Intersect(Range range)
    {
        if (Start > range.End || End < range.Start) return null;
        var start = Math.Max(Start, range.Start);
        var end = Math.Min(End, range.End);
        return new Range(start, end);
    }

    public Range LessThanOrEqual(long limit) =>
        limit < Start ? EMPTY : new(Math.Min(Start, limit), Math.Min(End, limit));

    public Range GreaterThanOrEqual(long limit) =>
        limit > End ? EMPTY : new(Math.Max(Start, limit), Math.Max(End, limit));

    public long Size() => End - Start + 1;
}

public static class Geometry
{

    public static double ShoelaceArea(this List<Coordinate> coordinates)
    {
        double left = 0.0;
        double right = 0.0;
        for (int i = 0; i < coordinates.Count; i++)
        {
            var a = coordinates[i];
            var b = coordinates[(i + 1) % coordinates.Count];
            left += a.X * b.Y;
            right += b.X * a.Y;
        }
        return 0.5 * Math.Abs(left - right);
    }

    public static double Perimeter(this List<Coordinate> coordinates)
    {
        double perimeter = 0.0;
        for (int i = 0; i < coordinates.Count; i++)
        {
            var a = coordinates[i];
            var b = coordinates[(i + 1) % coordinates.Count];
            perimeter += Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }
        return perimeter;
    }

    public static double NumberOfInternalPoints(this List<Coordinate> coordinates)
    {
        double area = coordinates.ShoelaceArea();
        double perimeter = coordinates.Perimeter();
        // From Pick's theorem: https://en.wikipedia.org/wiki/Pick%27s_theorem
        return area + 1 - perimeter / 2.0;
    }
}

public static class Dictionaries
{
    public static V ComputeIfAbsent<K, V>(this IDictionary<K, V> dict, K key, Func<K, V> valueFactory)
    {
        if (dict.TryGetValue(key, out var value)) return value;
        value = valueFactory(key);
        dict.Add(key, value);
        return value;
    }
}

public static class Maths
{
    public static long LCM(long a, long b)
    {
        return a / GCF(a, b) * b;
    }

    public static long GCF(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
}

public static class ArrayExtensions
{
    public static string GridToString<T>(this T[,] grid)
    {
        var sb = new StringBuilder();
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
                sb.Append(grid[y, x]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static char[,] ToGrid(this string[] lines)
    {
        char[,] grid = new char[lines.Length, lines[0].Length];
        for (int y = 0; y < grid.GetLength(0); y++)
            for (int x = 0; x < grid.GetLength(1); x++)
                grid[y, x] = lines[y][x];
        return grid;
    }

    public static Coordinate LastCoordinate<T>(this T[,] array)
    {
        return new(array.GetLength(1) - 1, array.GetLength(0) - 1);
    }

    public static bool ContainsCoordinate<T>(this T[][] array, Coordinate c)
    {
        return c.Y >= 0 && c.Y < array.Length && c.X >= 0 && c.X < array[c.Y].Length;
    }

    public static bool ContainsCoordinate<T>(this T[,] array, Coordinate c)
    {
        return c.Y >= 0 && c.Y < array.GetLength(0) && c.X >= 0 && c.X < array.GetLength(1);
    }

    public static T? ValueAt<T>(this T[][] array, Coordinate c)
    {
        return array.ContainsCoordinate(c)
            ? array[c.Y][c.X]
            : default;
    }

    public static T? ValueAt<T>(this T[,] array, Coordinate c)
    {
        return array.ContainsCoordinate(c)
            ? array[c.Y, c.X]
            : default;
    }

    public static Coordinate Wrapped<T>(this T[,] array, Coordinate c)
    {
        int w = array.GetLength(1);
        int h = array.GetLength(0);
        return new((c.X + w) % w, (c.Y + h) % h);
    }

    public static IEnumerable<Coordinate> Iterate2D<T>(this T[][] array)
    {
        for (int y = 0; y < array.Length; y++)
            for (int x = 0; x < array[y].Length; x++)
                yield return new(x, y);
    }

    public static IEnumerable<Coordinate> Iterate2D<T>(this T[,] array)
    {
        for (int y = 0; y < array.GetLength(0); y++)
            for (int x = 0; x < array.GetLength(1); x++)
                yield return new(x, y);
    }

    public static IEnumerable<T> Row<T>(this T[,] array, int row)
    {
        for (int i = 0; i < array.GetLength(1); i++)
            yield return array[row, i];
    }

    public static IEnumerable<T> Column<T>(this T[,] array, int column)
    {
        for (int i = 0; i < array.GetLength(0); i++)
            yield return array[i, column];
    }

    public static IEnumerable<Coordinate> NeighborsAndDiagonals<T>(this T[][] array, Coordinate c)
    {
        yield return c.NeighborWest();
        yield return c.NeighborNorthWest();
        yield return c.NeighborNorth();
        yield return c.NeighborNorthEast();
        yield return c.NeighborEast();
        yield return c.NeighborSouthEast();
        yield return c.NeighborSouth();
        yield return c.NeighborSouthWest();
    }

    public static IEnumerable<(T a, T b)> ZipWithNext<T>(this IEnumerable<T> source)
    {
        return source.Where((_, i) => i % 2 == 0)
            .Zip(source.Where((_, i) => i % 2 == 1),
            (a, b) => (a, b));
    }

    public static long LCM(this IEnumerable<long> source)
    {
        return source.Aggregate(Maths.LCM);
    }
}

public static class Graphs
{
    public static Dictionary<T, double> Dijkstra<T>(IEnumerable<T> vertices, T start, Func<T, IEnumerable<(T v, double d)>> neighbors)
    where T : notnull
    {
        var dist = new Dictionary<T, double>();
        var prev = new Dictionary<T, T>();
        var queue = vertices.ToHashSet();
        foreach (var v in vertices) dist[v] = double.PositiveInfinity;
        dist[start] = 0;

        while (queue.Count > 0)
        {
            var u = queue.OrderBy(v => dist[v]).First();
            queue.Remove(u);

            foreach (var (v, d) in neighbors(u).Where(e => queue.Contains(e.v)))
            {
                var alt = dist.GetValueOrDefault(u, double.PositiveInfinity) + d;
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        return dist;
    }
}

public abstract class AStar<T> where T : notnull
{
    private readonly Dictionary<T, T> cameFrom = [];

    public List<T> FindShortestPath(T start, Predicate<T> goal)
    {
        var openSet = new HashSet<T> { start };
        cameFrom.Clear();
        var gScore = new Dictionary<T, double> { { start, 0.0 } };
        var fScore = new Dictionary<T, double> { { start, Heuristic(start) } };

        while (openSet.Count > 0)
        {
            var current = openSet.MinBy(n => fScore.GetValueOrDefault(n, double.PositiveInfinity))!;
            if (goal.Invoke(current)) return ReconstructPath(current).Reverse().ToList();

            openSet.Remove(current);
            foreach (var neighbor in Neighbors(current))
            {
                var tentativeGScore = gScore.GetValueOrDefault(current, double.PositiveInfinity) + Distance(current, neighbor);
                if (tentativeGScore < gScore.GetValueOrDefault(neighbor, double.PositiveInfinity))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Heuristic(neighbor);
                    openSet.Add(neighbor);
                }
            }
        }
        throw new Exception("No path found");
    }

    protected IEnumerable<T> ReconstructPath(T current)
    {
        yield return current;
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            yield return current;
        }
    }

    protected abstract double Heuristic(T current);
    protected abstract IEnumerable<T> Neighbors(T current);
    protected abstract double Distance(T current, T neighbor);
}

// Because the semantics of BitVector32 are idiotic...
public class BitSet
{
    private int value;

    public static bool operator ==(BitSet a, BitSet b) => a.value == b.value;
    public static bool operator !=(BitSet a, BitSet b) => a.value != b.value;

    public bool this[int n]
    {
        get { return (value & (1 << n)) != 0; }
        set
        {
            if (value) this.value |= 1 << n;
            else this.value &= ~(1 << n);
        }
    }

    public override bool Equals(object? obj)
    {
        var o = obj as BitSet;
        return o is not null && value == o.value;
    }
    public override int GetHashCode() => value;
    public override string ToString() => Convert.ToString(value, 2);
}