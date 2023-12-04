namespace aoc2023.day4;

class Solution
{
    static void Main(string[] args)
    {
        var cards = File.ReadAllLines("inputs/day4.txt").Select(Card.Parse).ToList();
        var result1 = cards
        .Select(c => c.Score())
        .Sum();

        Console.WriteLine(result1);

        var cardCounts = cards.ToDictionary(c => c.CardNumber, _ => 1);
        foreach (var card in cards)
        {
            int currentCount = cardCounts[card.CardNumber];
            for (int i = 1; i <= card.Matches && card.CardNumber + i <= cards.Count; i++)
            {
                int nextCardNumber = card.CardNumber + i;
                cardCounts[nextCardNumber] += currentCount;
            }
        }

        var result2 = cardCounts.Sum(kv => kv.Value);
        Console.WriteLine(result2);
    }
}

record Card(int CardNumber, List<int> WinningNumbers, List<int> NumbersIHave)
{

    public static Card Parse(string input)
    {
        var parts = input.Split(['|', ':']);
        var cardNumber = parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(int.Parse).First();
        return new Card(cardNumber, ParseNumbers(parts[1]), ParseNumbers(parts[2]));
    }

    private static List<int> ParseNumbers(string input)
    {
        return input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
    }

    public int Matches = WinningNumbers.Intersect(NumbersIHave).Count();

    public int Score()
    {
        return Matches > 0 ? 1 << (Matches - 1) : 0;
    }

    public override int GetHashCode()
    {
        return CardNumber;
    }
}