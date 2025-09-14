namespace mediateur;

public interface IMyRequestHandler : IRequestHandler
{
    public int Sum(int i, int j);
}

public class MyRequestHandler : IMyRequestHandler
{
    public int Sum(int i, int j)
    {
        return i + j;
    }
}

public class AnotherMyRequestHandler : IMyRequestHandler
{
    public int Sum(int i, int j)
    {
        return i + j;
    }
}