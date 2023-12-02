namespace aoc2023;

class Solution1
{
    static readonly Func<string, IEnumerable<char>> Numbers = line => line.ToCharArray().Where(c => char.IsNumber(c));


    static void Main(string[] args)
    {
        var result = File.ReadAllLines("inputs/day1.txt")
        .Select(line => Numbers(line).First().ToString() + Numbers(line).Last().ToString())
        .Select(int.Parse)
        .Sum();

        Console.WriteLine(result);
    }
}