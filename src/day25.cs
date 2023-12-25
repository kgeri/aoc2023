namespace aoc2023.day25;

class Solution
{
    static void Main(string[] args)
    {
        // I solved this with yEd, lol :) (exported to .tgf, opened and layed it out in yEd, see day25.graphml)
        new Components(File.ReadAllLines("inputs/day25.txt")).ToTGF();
    }
}

class Components
{
    private readonly Dictionary<string, List<string>> graph = [];

    internal Components(string[] lines)
    {
        foreach (var line in lines)
        {
            var tokens = line.Split(": ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            graph.ComputeIfAbsent(tokens[0], _ => []).AddRange(tokens[1..]);
        }
    }

    public void ToTGF()
    {
        var nodes = new List<string>(graph.Keys);
        nodes.AddRange(graph.Values.SelectMany(v => v));
        Dictionary<string, int> ids = nodes.Distinct().Select((k, i) => (k, i + 1)).ToDictionary();
        foreach (var v in graph.Keys)
            Console.WriteLine($"{ids[v]} {v}");
        Console.WriteLine("#");
        foreach (var (v, ns) in graph)
            foreach (var n in ns)
                Console.WriteLine($"{ids[v]} {ids[n]}");
    }
}