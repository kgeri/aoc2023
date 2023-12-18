namespace aoc2023.day18;

class Solution
{
    static void Main(string[] args)
    {
        var instructions = File.ReadAllLines("inputs/day18.txt")
            // .Select(Instruction.Parse) // Part 1
            .Select(Instruction.Parse2) // Part 2
            .ToList();

        List<Coordinate> coordinates = [new(0, 0)];
        foreach (var ins in instructions)
            coordinates.Add(coordinates.Last().Neighbor(ins.Dir, ins.Steps));
        coordinates.RemoveAt(coordinates.Count - 1); // Last value should be the same as the first

        var result = coordinates.NumberOfInternalPoints() + coordinates.Perimeter();
        Console.WriteLine(result);
    }
}

record Instruction(Direction Dir, long Steps, string Color)
{
    public static Instruction Parse(string line)
    {
        var parts = line.Split(' ');
        var dir = parts[0] switch
        {
            "U" => Direction.North,
            "D" => Direction.South,
            "L" => Direction.West,
            "R" => Direction.East,
            _ => throw new NotImplementedException(),
        };
        return new(dir, int.Parse(parts[1]), parts[2][1..^1]);
    }

    public static Instruction Parse2(string line)
    {
        var parts = line.Split(' ');
        var distanceAndDir = parts[2][2..^1];
        long distance = Convert.ToInt32(distanceAndDir[0..5], 16);
        var dir = distanceAndDir[^1] switch
        {
            '3' => Direction.North,
            '1' => Direction.South,
            '2' => Direction.West,
            '0' => Direction.East,
            _ => throw new NotImplementedException(),
        };
        return new(dir, distance, "");
    }
}