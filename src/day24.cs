namespace aoc2023.day24;

class Solution
{
    static void Main(string[] args)
    {
        var tester = new HailstoneTester(File.ReadAllLines("inputs/day24.txt"));

        var result1 = tester.NumberOfIntersections();
        Console.WriteLine(result1);

        var rc = tester.CalculateIdealRockCoordinate();
        var result2 = rc.X + rc.Y + rc.Z;
        Console.WriteLine(result2);
    }
}

class HailstoneTester
{
    private readonly Hail[] hails;

    internal HailstoneTester(string[] lines)
    {
        hails = lines.Select(l => new Hail(l)).ToArray();
    }

    public int NumberOfIntersections()
    {
        Coordinate min = new(200000000000000, 200000000000000);
        Coordinate max = new(400000000000000, 400000000000000);
        // Coordinate min = new(7, 7);
        // Coordinate max = new(27, 17);

        int count = 0;
        for (int i = 1; i < hails.Length; i++)
        {
            Hail hi = hails[i];
            for (int j = 0; j < i; j++)
            {
                Hail hj = hails[j];
                var (px, py) = hi.IntersectsWith(hj);
                if (double.IsInfinity(px) || double.IsInfinity(py) // Parallel
                || px < min.X || px >= max.X // Out of bounds
                || py < min.Y || py >= max.Y
                || !hi.IsFuturePoint(px)
                || !hj.IsFuturePoint(px)) continue;

                // Debug :)
                // Console.WriteLine($"{hi} x {hj} = ({px},{py})");
                count++;
            }
        }

        return count;
    }

    public Coordinate3D CalculateIdealRockCoordinate()
    {
        // Disclaimer: not my idea, but at least one I understand and doesn't require the use of a linalg solver
        // Kudos to: https://www.reddit.com/user/Smooth-Aide-1751/
        Hail h1 = hails[0];
        Hail h2 = hails[1];

        int range = 500;
        for (int vx = -range; vx < range; vx++)
        {
            Console.WriteLine($"Progress: {(vx + range) * 50 / range}%");
            for (int vy = -range; vy < range; vy++)
                for (int vz = -range; vz < range; vz++)
                {
                    if (vx == 0 || vy == 0 || vz == 0) continue;

                    long A = h1.position.X, a = h1.speed.X - vx;
                    long B = h1.position.Y, b = h1.speed.Y - vy;
                    long C = h2.position.X, c = h2.speed.X - vx;
                    long D = h2.position.Y, d = h2.speed.Y - vy;

                    // skip if division by 0
                    if (c == 0 || (a * d) - (b * c) == 0) continue;

                    // Rock intercepts H1 at time t
                    long t = (d * (C - A) - c * (D - B)) / ((a * d) - (b * c));

                    // Calculate starting position of rock from intercept point
                    long x = h1.position.X + h1.speed.X * t - vx * t;
                    long y = h1.position.Y + h1.speed.Y * t - vy * t;
                    long z = h1.position.Z + h1.speed.Z * t - vz * t;

                    // check if this rock throw will hit all hailstones
                    bool hitall = hails.All(h =>
                    {
                        long hx = h.position.X, hy = h.position.Y, hz = h.position.Z;
                        long hvx = h.speed.X, hvy = h.speed.Y, hvz = h.speed.Z;
                        long u;
                        if (hvx != vx) u = (x - hx) / (hvx - vx);
                        else if (hvy != vy) u = (y - hy) / (hvy - vy);
                        else if (hvz != vz) u = (z - hz) / (hvz - vz);
                        else throw new NotSupportedException();
                        return (x + u * vx == hx + u * hvx) && (y + u * vy == hy + u * hvy) && (z + u * vz == hz + u * hvz);
                    });

                    if (hitall) return new(x, y, z);
                }
        }
        throw new Exception("No solution found");
    }
}

class Hail
{
    internal readonly Coordinate3D position;
    internal readonly Coordinate3D speed;

    internal Hail(string line)
    {
        var t = line.Split(" ,@".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
        position = new(t[0], t[1], t[2]);
        speed = new(t[3], t[4], t[5]);
    }

    public (double, double) IntersectsWith(Hail o)
    {
        static (double slope, double yintercept) getLineEquation(Hail h) =>
            ((double)h.speed.Y / h.speed.X, h.position.Y - (h.position.X * h.speed.Y / (double)h.speed.X));

        var (a1, b1) = getLineEquation(this);
        var (a2, b2) = getLineEquation(o);

        double ix = (b2 - b1) / (a1 - a2);
        double iy = a1 * ix + b1;
        return (ix, iy);
    }

    public bool IsFuturePoint(double ix) => (ix - position.X) / speed.X >= 0;

    public override string ToString() => $"{position}@{speed}";
}