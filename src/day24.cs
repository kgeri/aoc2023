namespace aoc2023.day24;

class Solution
{
    static void Main(string[] args)
    {
        var tester = new HailstoneTester(File.ReadAllLines("inputs/day24.txt"));

        var result1 = tester.NumberOfIntersections();
        Console.WriteLine(result1);
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
}

class Hail
{
    private readonly Coordinate3D position;
    private readonly Coordinate3D speed;

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