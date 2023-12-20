namespace aoc2023.day20;

class Solution
{
    static void Main(string[] args)
    {
        var evaluator = new ModuleEvaluator(File.ReadAllLines("inputs/day20.txt"));
        for (int i = 0; i < 1000; i++)
        {
            evaluator.PushButton();
        }
        var result1 = evaluator.GetPulseProduct();
        Console.WriteLine(result1);
    }
}

class ModuleEvaluator
{
    private readonly Dictionary<string, Module> modules;

    private readonly Dictionary<bool, long> pulses = [];

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

        pulses[true] = 0;
        pulses[false] = 0;
    }

    public void PushButton()
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

    public long GetPulseProduct() => pulses[true] * pulses[false];
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
    private readonly Dictionary<Module, bool> inputs = [];

    internal void AddInput(Module module) => inputs[module] = false;

    public override bool? Process(Module source, bool signal)
    {
        inputs[source] = signal; // When a pulse is received, the conjunction module first updates its memory for that input
        return !inputs.Values.All(v => v); // ...f it remembers high pulses for all inputs, it sends a low pulse; otherwise, it sends a high pulse
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