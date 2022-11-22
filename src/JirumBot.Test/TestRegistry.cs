using FluentScheduler;

namespace JirumBot.Lab;

public class TestRegistry: Registry
{
    public TestRegistry()
    {
        NonReentrantAsDefault();

        Schedule<Test1>().ToRunNow().AndEvery(3).Seconds();
        Schedule<Test2>().ToRunNow().AndEvery(3).Seconds();
        Schedule<Test3>().ToRunNow().AndEvery(3).Seconds();
    }
}

public class Test1 : IJob
{
    public void Execute()
    {
        Thread.Sleep(5000);
        Console.WriteLine(1);
    }
}

public class Test2 : IJob
{
    public void Execute()
    {
        Thread.Sleep(3000);
        Console.WriteLine(2);
    }
}


public class Test3 : IJob
{
    public void Execute()
    {
        Console.WriteLine(3);
    }
}