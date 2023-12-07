

namespace aoc2023.day7;

class Solution
{
    static void Main(string[] args)
    {
        var deals = File.ReadAllLines("inputs/day7.txt")
            .Select(Deal.Parse)
            .ToList();

        deals.Sort((d1, d2) => Hand.ByRank(d1.Hand, d2.Hand));
        var result1 = deals.Select((d, i) => d.Bid * (i + 1)).Sum();

        Console.WriteLine(result1);
    }
}

record Deal(Hand Hand, long Bid)
{
    internal static Deal Parse(string line)
    {
        var parts = line.Split(" ");
        var bid = int.Parse(parts[1]);
        var cards = parts[0].ToCharArray();
        return new(new(cards), bid);
    }
}

class Hand
{
    private static readonly Dictionary<char, int> Strength = "AKQJT98765432"
        .ToCharArray()
        .Select((c, i) => (c, i))
        .ToDictionary(it => it.c, it => 13 - it.i);

    private readonly char[] cards;
    private readonly int score;

    public Hand(char[] cards)
    {
        if (cards.Length != 5) throw new ArgumentException("A hand must have 5 cards");
        this.cards = cards;

        var groups = cards.GroupBy(c => c);
        if (groups.Count() == 1)
            score = 7; // Five of a kind
        else if (groups.Any(g => g.Count() == 4))
            score = 6; // Four of a kind
        else if (groups.Any(g => g.Count() == 3) && groups.Any(g => g.Count() == 2))
            score = 5; // Full house
        else if (groups.Any(g => g.Count() == 3))
            score = 4; // Three of a kind
        else if (groups.Count(g => g.Count() == 2) == 2)
            score = 3; // Two pair
        else if (groups.Count(g => g.Count() == 2) == 1)
            score = 2; // One pair
        else if (groups.Count() == 5)
            score = 1; // High card
        else
            throw new NotImplementedException($"Unknown hand: {new string(cards)}");
    }

    public override string ToString() => $"{new string(cards)} = {score}";

    internal static int ByRank(Hand a, Hand b)
    {
        if (a.score < b.score) return -1;
        else if (a.score > b.score) return 1;
        else
        {
            for (int i = 0; i < a.cards.Length; i++)
            {
                var ac = a.cards[i];
                var bc = b.cards[i];
                if (Strength[ac] < Strength[bc]) return -1;
                else if (Strength[ac] > Strength[bc]) return 1;
            }
            throw new NotImplementedException($"Impossible to order: {a} vs {b}");
        }
    }
}