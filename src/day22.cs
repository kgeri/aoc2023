
namespace aoc2023.day22;

class Solution
{
    static void Main(string[] args)
    {
        var bs = new BrickSimulator(File.ReadAllLines("inputs/day22.txt")
            .Select(l => new Brick(l)));
        bs.DropAll();

        // Part 1
        var result1 = bs.CalculateSafeToRemove();
        Console.WriteLine(result1);

        // Part 2
        var result2 = bs.CalculateChainReaction();
        Console.WriteLine(result2);
    }
}

class Brick
{
    private Coordinate3D c1;
    private Coordinate3D c2;

    internal Brick(string line)
    {
        var t = line.Split(['~', ',']).Select(long.Parse).ToArray();
        c1 = new(t[0], t[1], t[2]);
        c2 = new(t[3], t[4], t[5]);
    }

    internal Brick(Coordinate3D c1, Coordinate3D c2)
    {
        this.c1 = c1;
        this.c2 = c2;
    }

    public long MinX => Math.Min(c1.X, c2.X);
    public long MinY => Math.Min(c1.Y, c2.Y);
    public long MinZ => Math.Min(c1.Z, c2.Z);

    public long MaxX => Math.Max(c1.X, c2.X);
    public long MaxY => Math.Max(c1.Y, c2.Y);
    public long MaxZ => Math.Max(c1.Z, c2.Z);

    public void DropTo(long z)
    {
        long dz = z - MinZ;
        c1 = c1.Translate(dz: dz);
        c2 = c2.Translate(dz: dz);
    }

    public bool Intersects(Brick b) =>
        MinX <= b.MaxX &&
        MaxX >= b.MinX &&
        MaxY >= b.MinY &&
        MinY <= b.MaxY;
}

class BrickSimulator
{
    private static readonly Brick FLOOR = new(new(long.MinValue, long.MinValue, 0), new(long.MaxValue, long.MaxValue, 0));
    private readonly List<Brick> bricks;

    internal BrickSimulator(IEnumerable<Brick> bricks)
    {
        this.bricks = [.. bricks.OrderBy(b => b.MinZ).Prepend(FLOOR)];
    }

    public long Height => bricks[^1].MaxZ;

    public void DropAll()
    {
        for (int i = 1; i < bricks.Count; i++)
        {
            var brick = bricks[i];
            var maxZ = bricks.Take(i).Where(brick.Intersects).Max(b => b.MaxZ);
            brick.DropTo(maxZ + 1);
        }
    }

    public int CalculateSafeToRemove()
    {
        Dictionary<Brick, List<Brick>> supportedBy = CalculateSupports();

        int result = 0;
        for (int i = 1; i < bricks.Count; i++) // Skipping the floor
        {
            var brick = bricks[i];
            var supportedBricks = supportedBy[brick];
            if (supportedBricks.All(sb => supportedBy.Any(kv => kv.Key != brick && kv.Value.Contains(sb))))
            { // This brick supports nothing, or everything that it does is supported by some other brick
                result++;
            }
        }
        return result;
    }

    public int CalculateChainReaction()
    {
        Dictionary<Brick, List<Brick>> supportedBy = CalculateSupports();
        void calculateFallen(Brick brick, HashSet<Brick> fallen)
        {
            foreach (var sb in supportedBy[brick])
            {
                if (supportedBy.Any(kv => kv.Key != brick && !fallen.Contains(kv.Key) && kv.Value.Contains(sb)))
                    continue; // Something else still supports sb
                fallen.Add(sb);
                calculateFallen(sb, fallen);
            }
        }

        int count = 0;
        foreach (var brick in bricks)
        {
            if (brick == FLOOR) continue;
            HashSet<Brick> fallen = [];
            calculateFallen(brick, fallen);
            count += fallen.Count;
        }
        return count;
    }

    private Dictionary<Brick, List<Brick>> CalculateSupports()
    {
        Dictionary<Brick, List<Brick>> result = bricks.ToDictionary(b => b, _ => new List<Brick>());
        foreach (var top in bricks)
            foreach (var bottom in bricks)
                if (bottom.MaxZ + 1 == top.MinZ && bottom.Intersects(top))
                    result[bottom].Add(top);
        return result;
    }
}