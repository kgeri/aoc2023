using System.Security.Cryptography;
using System.Text;

namespace aoc2023.day14;

class Solution
{
    static void Main(string[] args)
    {
        var result1 = new Dish(File.ReadAllText("inputs/day14.txt").Split("\n", StringSplitOptions.RemoveEmptyEntries))
            .TiltNorth()
            .CalculateLoad();
        Console.WriteLine(result1);

        var result2 = new Dish(File.ReadAllText("inputs/day14.txt").Split("\n", StringSplitOptions.RemoveEmptyEntries))
            .CalculateLoadAfter(1000000000);
        Console.WriteLine(result2);
    }
}

class Dish
{
    private readonly char[,] grid;

    internal Dish(string[] lines)
    {
        grid = new char[lines.Length, lines[0].Length];
        for (int y = 0; y < grid.GetLength(0); y++)
            for (int x = 0; x < grid.GetLength(1); x++)
                grid[y, x] = lines[y][x];
    }

    public Dish TiltNorth()
    {
        for (int x = 0; x < grid.GetLength(1); x++)
        {
            int bottomY = -1;
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                switch (grid[y, x])
                {
                    case 'O':
                        if (y > bottomY + 1)
                        {
                            grid[bottomY + 1, x] = 'O';
                            grid[y, x] = '.';
                            bottomY++;
                        }
                        else
                        {
                            bottomY = y;
                        }
                        break;
                    case '#':
                        bottomY = y;
                        break;
                }
            }
        }
        return this;
    }

    public int CalculateLoad()
    {
        var load = 0;
        for (int y = 0; y < grid.GetLength(0); y++)
            load += grid.Row(y).Count(c => c == 'O') * (grid.GetLength(0) - y);
        return load;
    }

    public Dish RotateCW()
    {
        int n = grid.GetLength(0);
        for (int y = 0; y < n / 2; y++)
        {
            for (int j = y; j < n - y - 1; j++)
            {
                var temp = grid[y, j];
                grid[y, j] = grid[n - 1 - j, y];
                grid[n - 1 - j, y] = grid[n - 1 - y, n - 1 - j];
                grid[n - 1 - y, n - 1 - j] = grid[j, n - 1 - y];
                grid[j, n - 1 - y] = temp;
            }
        }
        return this;
    }

    public Signature CalculateSignature()
    {
        var sha = SHA1.Create();
        var coords = grid.Iterate2D().Where(c => grid.ValueAt(c) == 'O').Select(c => (byte)(c.X * c.Y)).ToArray();
        return new(coords);
    }

    internal int CalculateLoadAfter(int cycles)
    {
        var signatureToCycle = new Dictionary<Signature, int>();
        for (int c = 0; c < cycles; c++)
        {
            TiltNorth(); // Roll north
            RotateCW().TiltNorth(); // Roll west
            RotateCW().TiltNorth(); // Roll south
            RotateCW().TiltNorth(); // Roll east
            RotateCW(); // Rotate back to north

            // The idea is that there's a finite (as it turns out very small) number of cycles before 
            // we get the exact same arrangement. By hashing and storing the coordinates of the current
            // cycle, we can determine this cycle time, and fast-forward the calculation to that point
            var signature = CalculateSignature();
            if (signatureToCycle.TryGetValue(signature, out int oldCycle))
            {
                Console.WriteLine($"Found cycle {c} == {oldCycle}");
                var repetitionLength = c - oldCycle; // The pattern repeats itself after this many cycles
                c = cycles - (cycles - c) % repetitionLength; // Skipping a whole lot of cycles
            }
            else
            {
                signatureToCycle[signature] = c;
            }
        }
        return CalculateLoad();
    }

    public override string ToString()
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
}

class Signature
{
    private readonly byte[] value;

    internal Signature(byte[] value)
    {
        this.value = value;
    }

    public override bool Equals(object? obj)
    {
        var o = obj as Signature;
        return o is not null && value.SequenceEqual(o.value);
    }

    public override int GetHashCode()
    {
        return BitConverter.ToInt32(value);
    }
}