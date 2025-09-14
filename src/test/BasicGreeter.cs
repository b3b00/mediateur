

using interfaces;

namespace mediateurtest;

public class ObsequiousGreeter : IGreeter
{
    public void Greet(string name)
    {
        Console.WriteLine($"Je vous salue bien bas, {name}!");
    }
}

public class BasicGreeter : IGreeter
{
    public void Greet(string name)
    {
        Console.WriteLine($"Bonjour, {name} !");
    }
}

