

namespace aoc2023.day7;

class Solution
{
    static void Main(string[] args)
    {
        var deals = File.ReadAllLines("inputs/day7.txt")
            .Select(Deal.Parse)
            .ToList();

        deals.Sort((d1, d2) => Hand.ByRank(d1.Hand, d2.Hand));
        var result2 = deals.Select((d, i) => d.Bid * (i + 1)).Sum();

        Console.WriteLine(result2);
    }
}

record Deal(Hand Hand, long Bid)
{
    internal static Deal Parse(string line)
    {
        var parts = line.Split(" ");
        var bid = int.Parse(parts[1]);
        return new(new(parts[0]), bid);
    }
}

class Hand
{
    private static readonly Dictionary<char, int> Strength = "AKQT98765432J" // Note: moved J at the end for Part 2
        .ToCharArray()
        .Select((c, i) => (c, i))
        .ToDictionary(it => it.c, it => 13 - it.i);

    private readonly string cards;
    private readonly int score;

    public Hand(string cards)
    {
        if (cards.Length != 5) throw new ArgumentException("A hand must have 5 cards");
        this.cards = cards;

        // Part 2: replacing J with whatever's the most common
        string replaced;
        if (cards == "JJJJJ")
        {
            // All are Js - replacing with highest card
            replaced = "AAAAA";
        }
        else
        {
            // At least one non-J - replacing with the most common other card
            var mostCommon = cards.GroupBy(c => c)
                .Where(g => g.Key != 'J')
                .OrderByDescending(g => g.Count())
                .First().Key;
            replaced = cards.Replace('J', mostCommon);
        }
        // end of Part 2 hack

        var groups = replaced.GroupBy(c => c);
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
            throw new NotImplementedException($"Unknown hand: {new string(replaced)}");
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
            return 0;
        }
    }
}