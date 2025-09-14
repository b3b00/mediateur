namespace mediateur;

public interface IGreeter : INotificationHandler
{
    void Greet(string name);
}