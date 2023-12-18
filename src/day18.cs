namespace aoc2023.day18;

class Solution
{
    static void Main(string[] args)
    {
        var instructions = File.ReadAllLines("inputs/day18.txt")
            .Select(Instruction.Parse)
            .ToList();

        List<Coordinate> coordinates = [new(0, 0)];
        foreach (var ins in instructions)
            coordinates.Add(coordinates.Last().Neighbor(ins.Dir, ins.Steps));
        coordinates.RemoveAt(coordinates.Count - 1); // Last value should be the same as the first

        var result1 = coordinates.NumberOfInternalPoints() + coordinates.Perimeter();
        Console.WriteLine(result1);
    }
}

record Instruction(Direction Dir, int Steps, string Color)
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
}