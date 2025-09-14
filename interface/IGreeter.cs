

using mediateur;

namespace interfaces;

public interface IGreeter : INotificationHandler
{
    void Greet(string name);
    
    void SayBye(string name);
}
