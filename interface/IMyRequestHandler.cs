namespace mediateur;

public interface IMyRequestHandler : IRequestHandler
{
    public int Sum(int i, int j);
}