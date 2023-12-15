using System.Text;

namespace aoc2023.day15;

class Solution
{
    static void Main(string[] args)
    {
        var result1 = File.ReadAllText("inputs/day15.txt").Split(['\n', ','])
            .Select(Hash)
            .Sum();
        Console.WriteLine(result1);

        BoxMap boxes = new();
        foreach (var ins in File.ReadAllText("inputs/day15.txt").Split(['\n', ',']))
        {
            var parts = ins.Split('=');
            if (parts.Length == 2) boxes.Put(parts[0], int.Parse(parts[1]));
            else boxes.Remove(parts[0].Replace("-", ""));
        }
        var result2 = boxes.FocusingPower();
        Console.WriteLine(result2);
    }

    public static int Hash(string input)
    {
        int current = 0;
        foreach (var v in Encoding.ASCII.GetBytes(input))
        {
            current += v;
            current *= 17;
            current %= 256;
        }
        return current;
    }
}

class BoxMap
{
    private readonly Box?[] boxes;

    internal BoxMap()
    {
        boxes = new Box[256];
    }

    public void Put(string label, int value)
    {
        int idx = Solution.Hash(label);
        Box? box = boxes[idx];
        if (box == null) boxes[idx] = new Box(label, value);
        else box.Put(label, value);
    }

    public void Remove(string label)
    {
        int idx = Solution.Hash(label);
        Box? box = boxes[idx];
        if (box == null) return;
        else boxes[idx] = box.Remove(label);
    }

    public int FocusingPower() => boxes.Select((box, i) => box == null ? 0 : (1 + i) * box.FocusingPower()).Sum();
}

class Box
{
    private readonly string label;
    private int value;
    private Box? next = null;

    internal Box(string label, int value)
    {
        this.label = label;
        this.value = value;
    }

    internal int FocusingPower()
    {
        Box current = this;
        for (int i = 1, power = 0; ; i++)
        {
            power += i * current.value;
            if (current.next == null) return power;
            current = current.next;
        }
    }

    internal void Put(string label, int value)
    {
        Box? current = this;
        Box last = this;
        while (current != null)
        {
            if (current.label == label)
            {
                current.value = value; // Box found and value set
                return;
            }
            last = current;
            current = current.next;
        }
        last.next = new Box(label, value); // Not found, adding new one
    }

    internal Box? Remove(string label)
    {
        Box? current = this;
        Box? last = this;
        while (current != null)
        {
            if (current.label == label)
            {
                if (current == this) return current.next; // Box removed from the start
                last.next = current.next; // Box removed from the middle
                return this;
            }
            last = current;
            current = current.next;
        }
        return this; // Not found, box list is unchanged
    }
}