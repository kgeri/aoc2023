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
    }
}

class RuleEvaluator
{
    private readonly Dictionary<string, SwitchRule> rules;

    internal RuleEvaluator(string lines)
    {
        rules = lines.Split('\n').Select(l => new SwitchRule(l)).ToDictionary(r => r.id, r => r);
    }

    public bool Accepts(Part part)
    {
        SwitchRule current = rules["in"];
        while (true)
        {
            string nextRuleId = current.Evaluate(part);
            if (nextRuleId == "A") return true;
            else if (nextRuleId == "R") return false;
            current = rules[nextRuleId];
        }
    }
}

class SwitchRule
{
    public readonly string id;
    private readonly List<(Predicate<Part> predicate, string nextRule)> rules = [];

    internal SwitchRule(string line)
    {
        var parts = line.Split(['{', '}']);
        id = parts[0];
        rules = parts[1].Split(',').Select(ParseRule).ToList();
    }

    private (Predicate<Part> predicate, string nextRule) ParseRule(string rule)
    {
        var parts = Regex.Match(rule, @"(\w+)([<>])(\d+):(\w+)").Groups;
        if (parts.Count == 1) // Default rule
            return (_ => true, rule);

        var field = parts[1].Value;
        var op = parts[2].Value;
        int value = int.Parse(parts[3].Value);
        string nextRule = parts[4].Value;

        Predicate<Part> predicate = field switch
        {
            "x" => op == "<"
                    ? (p) => p.X < value
                    : (p) => p.X > value,
            "m" => op == "<"
                    ? (p) => p.M < value
                    : (p) => p.M > value,
            "a" => op == "<"
                    ? (p) => p.A < value
                    : (p) => p.A > value,
            "s" => op == "<"
                    ? (p) => p.S < value
                    : (p) => p.S > value,
            _ => throw new NotSupportedException(field)
        };
        return (predicate, nextRule);
    }

    public string Evaluate(Part part)
    {
        foreach (var (predicate, nextRule) in rules)
            if (predicate.Invoke(part)) return nextRule;
        throw new NotImplementedException($"Failed to match: {part}");
    }
}

record Part(int X, int M, int A, int S)
{
    internal static Part Parse(string line)
    {
        var values = Regex.Match(line, @"{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}").Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
        return new(values[0], values[1], values[2], values[3]);
    }

    public int Rating() => X + M + A + S;
}
