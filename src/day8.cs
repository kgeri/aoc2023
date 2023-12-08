


namespace aoc2023.day8;

class Solution
{
    static void Main(string[] args)
    {
        var lines = File.ReadAllLines("inputs/day8.txt");
        var instructions = lines[0];
        var nodes = lines[2..].Select(Node.Parse);
        var nameToNode = nodes.ToDictionary(node => node.Name, node => node);

        var current = nameToNode["AAA"];
        int step = 0;
        for (; current.Name != "ZZZ"; step++)
        {
            switch (instructions[step % instructions.Length])
            {
                case 'L':
                    current = nameToNode[current.Left];
                    break;
                case 'R':
                    current = nameToNode[current.Right];
                    break;
            }
        }


        Console.WriteLine(step);
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