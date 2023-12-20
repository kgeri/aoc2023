using System.Data;

namespace aoc2023.day20;

class Solution
{
    static void Main(string[] args)
    {
        var evaluator = new ModuleEvaluator(File.ReadAllLines("inputs/day20.txt"));

        // Part 1
        // var result1 = evaluator.GetPulseProduct();
        // Console.WriteLine(result1);

        // Part 2
        var result2 = evaluator.CalculateButtonPressesForRxLow();
        Console.WriteLine(result2);
    }
}

class ModuleEvaluator
{
    private readonly Dictionary<string, Module> modules;

    internal ModuleEvaluator(string[] lines)
    {
        static string getName(string value) => value.Replace("%", "").Replace("&", "");

        modules = lines.Select<string, Module>(line =>
        {
            var name = line.Split(" -> ")[0];
            return name[0] switch
            {
                '%' => new FlipFlop(getName(name)),
                '&' => new Conjunction(getName(name)),
                'b' => new Broadcast(),
                _ => throw new NotImplementedException(name)
            };
        })
        .ToDictionary(m => m.Name, m => m);

        foreach (var line in lines)
        {
            var moduleParts = line.Split(" -> ");
            var name = getName(moduleParts[0]);
            var module = modules[name];
            var outputs = moduleParts[1].Split(",", StringSplitOptions.TrimEntries);
            foreach (var outputName in outputs)
            {
                var output = modules.ComputeIfAbsent(outputName, n => new Output()); // Defaulting unnamed modules to Outputs
                module.AddOutput(output);
                if (output is Conjunction c) c.AddInput(module);
            }
        }
    }

    public long GetPulseProduct()
    {
        Dictionary<bool, long> pulses = [];
        pulses[true] = 0;
        pulses[false] = 0;
        for (int i = 0; i < 1000; i++)
        {
            pulses[false]++; // The initial button press

            Queue<(Module, bool)> steps = [];
            steps.Enqueue((modules["broadcaster"], false));

            while (steps.TryDequeue(out (Module module, bool signal) step))
            {
                foreach (var om in step.module.outputs)
                {
                    // Debug :)
                    // Console.WriteLine($"{step.module} -{(step.signal ? "high" : "low")}-> {om}");

                    pulses[step.signal]++;
                    bool? nextSignal = om.Process(step.module, step.signal);
                    if (nextSignal is null) continue; // Terminated on a FlipFlop
                    steps.Enqueue((om, (bool)nextSignal));
                }
            }
        }
        return pulses[true] * pulses[false];
    }

    internal long CalculateButtonPressesForRxLow()
    {
        var rx = modules["rx"];
        Conjunction rxParent = (Conjunction)modules.Values.Where(m => m.outputs.Contains(rx)).First();
        HashSet<Module> monitored = [.. rxParent.inputs.Keys];
        List<long> firsts = [];

        for (int i = 1; monitored.Count > 0; i++)
        {
            Queue<(Module, bool)> steps = [];
            steps.Enqueue((modules["broadcaster"], false));

            while (steps.TryDequeue(out (Module module, bool signal) step))
            {
                foreach (var om in step.module.outputs)
                {
                    // Debug :)
                    // Console.WriteLine($"{step.module} -{(step.signal ? "high" : "low")}-> {om}");

                    if (step.signal && om == rxParent && monitored.Remove(step.module))
                        firsts.Add(i);

                    bool? nextSignal = om.Process(step.module, step.signal);
                    if (nextSignal is null) continue; // Terminated on a FlipFlop

                    steps.Enqueue((om, (bool)nextSignal));
                }
            }
        }

        return firsts.Aggregate(Maths.LCM);
    }
}

abstract class Module(string name)
{
    internal readonly List<Module> outputs = [];

    internal string Name
    {
        get { return name; }
    }

    internal virtual void AddOutput(Module module) => outputs.Add(module);

    public abstract bool? Process(Module source, bool signal);

    public override string ToString() => name;
}

class FlipFlop(string name) : Module(name)
{
    private bool state = false;

    public override bool? Process(Module source, bool signal)
    {
        if (signal) return null; // If a flip-flop module receives a high pulse, it is ignored and nothing happens
        return state = !state; // If it was off, it turns on and vice versa
    }
}

class Conjunction(string name) : Module(name)
{
    internal readonly Dictionary<Module, bool> inputs = [];

    internal void AddInput(Module module) => inputs[module] = false;

    public override bool? Process(Module source, bool signal)
    {
        inputs[source] = signal; // When a pulse is received, the conjunction module first updates its memory for that input
        return !inputs.Values.All(v => v); // ...if it remembers high pulses for all inputs, it sends a low pulse; otherwise, it sends a high pulse
    }
}

class Broadcast() : Module("broadcaster")
{
    public override bool? Process(Module source, bool signal) => signal;
}

class Output() : Module("output")
{
    public override bool? Process(Module source, bool signal) { return null; }
}