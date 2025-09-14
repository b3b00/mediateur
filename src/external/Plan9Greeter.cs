using interfaces;
using mediateur;

namespace external;

public class Plan9Greeter :IGreeter
{
    public void Greet(string name)
    {
        Console.WriteLine($"hello from plan 9, {name}");
    }
    
}