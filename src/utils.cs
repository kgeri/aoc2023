namespace aoc2023;

public record Coordinate(int X, int Y) { }

public static class ArrayExtensions
{
    public static T? ValueAt<T>(this T[][] array, Coordinate c)
    {
        return c.Y >= 0 && c.Y < array.Length && c.X >= 0 && c.X < array[c.Y].Length
            ? array[c.Y][c.X]
            : default;
    }

    public static IEnumerable<Coordinate> Iterate2D<T>(this T[][] array)
    {
        for (int y = 0; y < array.Length; y++)
        {
            for (int x = 0; x < array[y].Length; x++)
            {
                yield return new(x, y);
            }
        }
    }

    public static IEnumerable<Coordinate> NeighborsAndDiagonals<T>(this T[][] array, Coordinate c)
    {
        yield return new(c.X - 1, c.Y);
        yield return new(c.X - 1, c.Y - 1);
        yield return new(c.X, c.Y - 1);
        yield return new(c.X + 1, c.Y - 1);
        yield return new(c.X + 1, c.Y);
        yield return new(c.X + 1, c.Y + 1);
        yield return new(c.X, c.Y + 1);
        yield return new(c.X - 1, c.Y + 1);
    }

    public static IEnumerable<(T a, T b)> ZipWithNext<T>(this IEnumerable<T> source)
    {
        return source.Where((_, i) => i % 2 == 0)
            .Zip(source.Where((_, i) => i % 2 == 1),
            (a, b) => (a, b));
    }
}