namespace aoc2023.day8;

class Solution
{
    static void Main(string[] args)
    {
        var map = NodeMap.Parse(File.ReadAllLines("inputs/day8.txt"));
        var result2 = map.TotalStepsToEnd();
        Console.WriteLine(result2);
    }
}

record NodeMap(char[] Instructions, Dictionary<string, Node> NameToNode)
{
    internal static NodeMap Parse(string[] lines)
    {
        var instructions = lines[0].ToCharArray();
        var nodes = lines[2..].Select(Node.Parse);
        var nameToNode = nodes.ToDictionary(node => node.Name, node => node);
        return new(instructions, nameToNode);
    }

    public long TotalStepsToEnd()
    {
        var startNodes = NameToNode.Where(kvp => kvp.Key.EndsWith('A')).Select(kvp => kvp.Value);
        return startNodes
            .Select(StepsToEnd)
            .LCM();
    }

    private long StepsToEnd(Node current)
    {
        for (long i = 0; ; i++)
        {
            if (current.Name.EndsWith('Z')) return i;
            switch (Instructions[i % Instructions.Length])
            {
                case 'L':
                    current = NameToNode[current.Left];
                    break;
                case 'R':
                    current = NameToNode[current.Right];
                    break;
            }
        }
    }
}

record Node(string Name, string Left, string Right)
{
    internal static Node Parse(string line)
    {
        var separatorChars = " =(,)".ToCharArray();
        var tokens = line.Split(separatorChars, StringSplitOptions.RemoveEmptyEntries);
        return new(tokens[0], tokens[1], tokens[2]);
    }
}