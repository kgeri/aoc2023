using System.Text;

namespace aoc2023.day15;

class Solution
{
    static void Main(string[] args)
    {
        var result1 = File.ReadAllText("inputs/day15.txt").Split(['\n', ','])
            .Select(Hash)
            .Sum();
        Console.WriteLine(result1);
    }

    public static int Hash(string input)
    {
        int current = 0;
        foreach (var v in Encoding.ASCII.GetBytes(input))
        {
            current += v;
            current *= 17;
            current %= 256;
        }
        return current;
    }
}
