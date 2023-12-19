using System.Text.RegularExpressions;

namespace aoc2023.day19;

class Solution
{
    static void Main(string[] args)
    {
        var sections = File.ReadAllText("inputs/day19.txt").Split("\n\n");
        var evaluator = new RuleEvaluator(sections[0]);
        var parts = sections[1].Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(Part.Parse).ToList();

        var result1 = parts.Where(evaluator.Accepts).Select(p => p.Rating()).Sum();
        Console.WriteLine(result1);

        var result2 = evaluator.Combinations(new(1, 4000));
        Console.WriteLine(result2);
    }
}

class RuleEvaluator
{
    private readonly Dictionary<string, SwitchRule> rules;

    internal RuleEvaluator(string lines)
    {
        rules = lines.Split('\n').Select(l => new SwitchRule(l)).ToDictionary(r => r.id, r => r);
    }

    public bool Accepts(Part part) => Combinations(new(part)) > 0;

    public long Combinations(PartRanges ranges)
    {
        long combinations = 0;
        Stack<(PartRanges input, string nextRule)> stack = [];
        stack.Push((ranges, "in"));

        while (stack.TryPop(out (PartRanges input, string nextRule) entry))
        {
            if (entry.nextRule == "A")
            {
                combinations += entry.input.Size();
                continue;
            }
            else if (entry.nextRule == "R") continue;

            foreach (var newEntry in rules[entry.nextRule].Evaluate(entry.input))
                stack.Push(newEntry);
        }

        return combinations;
    }
}

class SwitchRule
{
    public readonly string id;
    private readonly List<(Rule rule, string nextRule)> rules = [];

    internal SwitchRule(string line)
    {
        var parts = line.Split(['{', '}']);
        id = parts[0];
        rules = parts[1].Split(',').Select(ParseRule).ToList();
    }

    private (Rule rule, string nextRule) ParseRule(string rule)
    {
        var parts = Regex.Match(rule, @"(\w+)([<>])(\d+):(\w+)").Groups;
        if (parts.Count == 1) // Default rule
            return (new Rule("", ' ', 0), rule);

        var field = parts[1].Value;
        var op = parts[2].Value;
        int value = int.Parse(parts[3].Value);
        string nextRule = parts[4].Value;

        return (new(field, op[0], value), nextRule);
    }

    public IEnumerable<(PartRanges, string)> Evaluate(PartRanges input)
    {
        var remainder = input;
        foreach (var ((field, op, limit), nextRule) in rules)
        {
            if (remainder.IsEmpty()) yield break; // When there's nothing left, we quit
            else if (op == ' ') { yield return (remainder, nextRule); break; }

            // Apply the operation to the selected range, return that and update the remainder
            Range range = remainder[field];
            if (op == '<')
            {
                var output = remainder.With(field, range.LessThanOrEqual(limit - 1));
                yield return (output, nextRule);
                remainder = remainder.With(field, range.GreaterThanOrEqual(limit));
            }
            else if (op == '>')
            {
                var output = remainder.With(field, range.GreaterThanOrEqual(limit + 1));
                yield return (output, nextRule);
                remainder = remainder.With(field, range.LessThanOrEqual(limit));
            }
        }
    }
}

record Rule(string Field, char Op, int Limit);

record Part(int X, int M, int A, int S)
{
    internal static Part Parse(string line)
    {
        var values = Regex.Match(line, @"{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}").Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
        return new(values[0], values[1], values[2], values[3]);
    }

    public int Rating() => X + M + A + S;
}

class PartRanges
{
    private readonly Dictionary<string, Range> ranges = [];

    public PartRanges(Part part)
    {
        ranges["x"] = new(part.X, part.X);
        ranges["m"] = new(part.M, part.M);
        ranges["a"] = new(part.A, part.A);
        ranges["s"] = new(part.S, part.S);
    }

    public PartRanges(int from, int to)
    {
        ranges["x"] = new(from, to);
        ranges["m"] = new(from, to);
        ranges["a"] = new(from, to);
        ranges["s"] = new(from, to);
    }

    private PartRanges(Dictionary<string, Range> ranges)
    {
        this.ranges = ranges;
    }

    public Range this[string name]
    {
        get { return ranges[name]; }
    }

    public PartRanges With(string field, Range range)
    {
        Dictionary<string, Range> rs = new(ranges)
        {
            [field] = range
        };
        return new PartRanges(rs);
    }

    public bool IsEmpty() => ranges.Values.Any(v => v.Size() == 0);

    public long Size() => ranges.Values.Aggregate(1L, (acc, v) => acc * v.Size());
}
