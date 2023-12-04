namespace aoc2023.day4;

class Solution
{
    static void Main(string[] args)
    {
        var result1 = File.ReadAllLines("inputs/day4.txt")
        .Select(Card.Parse)
        .Select(c => c.Score())
        .Sum();

        Console.WriteLine(result1);
    }
}

record Card(List<int> WinningNumbers, List<int> NumbersIHave)
{

    public static Card Parse(string input)
    {
        var parts = input.Split(['|', ':']);
        return new Card(ParseNumbers(parts[1]), ParseNumbers(parts[2]));
    }

    private static List<int> ParseNumbers(string input)
    {
        return input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    }

    public int Score()
    {
        var matches = WinningNumbers.Intersect(NumbersIHave).Count();
        return matches > 0 ? 1 << (matches - 1) : 0;
    }
}