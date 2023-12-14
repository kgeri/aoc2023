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