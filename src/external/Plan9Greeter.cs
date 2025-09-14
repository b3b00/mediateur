using System.Diagnostics.CodeAnalysis;
using interfaces;
using mediateur;

namespace external;



public class Plan9Greeter :IGreeter
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Plan9Greeter))]
    public void Greet(string name)
    {
        Console.WriteLine($"hello from plan 9, {name}");
    }
    
    public void SayBye(string name)
    {
        Console.WriteLine($"bye bye, {name}");
    }
    
}