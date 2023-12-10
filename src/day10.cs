using System.ComponentModel.DataAnnotations;
using System.Text;

namespace aoc2023.day10;

class Solution
{
    static void Main(string[] args)
    {
        var grid = new PipeGrid(File.ReadAllLines("inputs/day10.txt"));
        Console.WriteLine(grid);

        var result1 = grid.StepsToFurthest();
        Console.WriteLine(result1);

        var result2 = grid.AreaInside(grid.Loop()).Count;
        Console.WriteLine(result2);
    }
}

class Node(char type, Coordinate coordinate)
{
    internal readonly Coordinate Coordinate = coordinate;
    internal readonly char Type = type;
    private Node? N;
    private Node? S;
    private Node? E;
    private Node? W;

    internal IEnumerable<Node> Neighbors()
    {
        if (N != null) yield return N;
        if (S != null) yield return S;
        if (E != null) yield return E;
        if (W != null) yield return W;
    }

    internal void LinkNorth(Node? n)
    {
        if (n != null && n.LinksS())
        {
            N = n;
            n.S = this;
        }
    }

    internal void LinkSouth(Node? s)
    {
        if (s != null && s.LinksN())
        {
            S = s;
            s.N = this;
        }
    }

    internal void LinkEast(Node? e)
    {
        if (e != null && e.LinksW())
        {
            E = e;
            e.W = this;
        }
    }

    internal void LinkWest(Node? w)
    {
        if (w != null && w.LinksE())
        {
            W = w;
            w.E = this;
        }
    }

    private bool LinksN() => "|LJ".Contains(Type);
    private bool LinksE() => "-LF".Contains(Type);
    private bool LinksW() => "-J7".Contains(Type);
    private bool LinksS() => "|7F".Contains(Type);
}

class PipeGrid
{
    private readonly List<Node> nodes = [];
    private readonly Node start;

    internal PipeGrid(string[] lines)
    {
        var grid = new Node[lines.Length, lines[0].Length];
        for (int y = 0; y < lines.Length; y++)
            for (int x = 0; x < lines[y].Length; x++)
                grid[y, x] = new(lines[y][x], new(x, y));

        foreach (var c in grid.Iterate2D())
        {
            var node = grid.ValueAt(c)!;
            var n = grid.ValueAt(c.NeighborNorth());
            var s = grid.ValueAt(c.NeighborSouth());
            var e = grid.ValueAt(c.NeighborEast());
            var w = grid.ValueAt(c.NeighborWest());

            switch (node.Type)
            {
                case '|':
                    node.LinkNorth(n);
                    node.LinkSouth(s);
                    break;
                case '-':
                    node.LinkEast(e);
                    node.LinkWest(w);
                    break;
                case 'L':
                    node.LinkNorth(n);
                    node.LinkEast(e);
                    break;
                case 'J':
                    node.LinkNorth(n);
                    node.LinkWest(w);
                    break;
                case '7':
                    node.LinkWest(w);
                    node.LinkSouth(s);
                    break;
                case 'F':
                    node.LinkEast(e);
                    node.LinkSouth(s);
                    break;
                case '.': break;
                case 'S':
                    start = node;
                    node.LinkNorth(n);
                    node.LinkSouth(s);
                    node.LinkEast(e);
                    node.LinkWest(w);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid pipe type: {node.Type}");
            }

            if (node.Type != '.') nodes.Add(node);
        }

        if (start == null) throw new InvalidOperationException("Start node not found");
    }

    public double StepsToFurthest()
    {
        var dist = Graphs.Dijkstra(nodes, start, n => n.Neighbors().Select(n => (n, 1.0)));
        return dist.Select(d => d.Value).Where(d => d != double.PositiveInfinity).Max();
    }

    public List<Node> Loop()
    {
        List<Node> result = [];
        Node current = start;
        Node previous = start;
        do
        {
            result.Add(current);
            var next = current.Neighbors().Where(n => n != previous).First();
            previous = current;
            current = next;
        } while (current != start);
        return result;
    }

    internal List<Coordinate> AreaInside(List<Node> nodeLoop)
    {
        List<Coordinate> loop = nodeLoop.Select(n => n.Coordinate).ToList();
        var max = loop.Aggregate((c1, c2) => new(Math.Max(c1.X, c2.X), Math.Max(c1.Y, c2.Y)));

        List<Coordinate> area = [];
        for (int y = -1; y < max.Y + 1; y++)
        {
            for (int x = -1; x < max.X + 1; x++)
            {
                Coordinate c = new(x, y);
                if (loop.Contains(c)) continue;
                if (c.IsInside(loop)) area.Add(c);
            }
        }
        return area;
    }

    // For fun (and debugging :) )
    public override string ToString()
    {
        static char prettify(char c) => c switch
        {
            'L' => '└',
            'J' => '┘',
            '7' => '┐',
            'F' => '┌',
            'S' => '▫',
            '|' => '│',
            '-' => '─',
            _ => c
        };

        var areaInside = AreaInside(Loop()).ToHashSet();

        Coordinate max = nodes.Select(n => n.Coordinate).Aggregate((c1, c2) => new Coordinate(Math.Max(c1.X, c2.X), Math.Max(c1.Y, c2.Y)));
        var sb = new StringBuilder();
        for (int y = 0; y < max.Y + 1; y++)
        {
            for (int x = 0; x < max.X + 1; x++)
            {
                var type = nodes.Where(n => n.Coordinate.X == x && n.Coordinate.Y == y).Select(n => prettify(n.Type)).FirstOrDefault(' ');
                if (areaInside.Contains(new(x, y))) type = '█';
                sb.Append(type);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}