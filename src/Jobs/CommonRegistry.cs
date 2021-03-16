using FluentScheduler;

namespace JirumBot.Jobs
{
    public class CommonRegistry : Registry
    {
        internal CommonRegistry()
        {
            Schedule<CoolJob>().ToRunNow().AndEvery(10).Seconds();
            Schedule<QuasarJob>().ToRunNow().AndEvery(10).Seconds();
        }
    }
}