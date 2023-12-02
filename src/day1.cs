using System.Text.RegularExpressions;

namespace aoc2023.day1;

partial class Solution
{
    static readonly string NumberExpr = @"(\d|one|two|three|four|five|six|seven|eight|nine)";
    static int ToNumber(string line, bool first)
    {
        Regex regex = new(NumberExpr, first ? RegexOptions.None : RegexOptions.RightToLeft);
        return regex.Match(line).Groups[1].Value switch
        {
            "one" => 1,
            "two" => 2,
            "three" => 3,
            "four" => 4,
            "five" => 5,
            "six" => 6,
            "seven" => 7,
            "eight" => 8,
            "nine" => 9,
            string d => int.Parse(d)
        };
    }

    static void Main(string[] args)
    {
        var result = File.ReadAllLines("inputs/day1.txt")
        .Select(line => ToNumber(line, true).ToString() + ToNumber(line, false))
        .Select(int.Parse)
        .Sum();

        Console.WriteLine(result);
    }
}