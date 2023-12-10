namespace aoc2023;

public record Coordinate(int X, int Y)
{
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
        return (a / GCF(a, b)) * b;
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