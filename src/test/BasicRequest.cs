using interfaces;


namespace mediateurtest;

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
