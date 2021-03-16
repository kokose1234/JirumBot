using FluentScheduler;
using JirumBot.Data;

namespace JirumBot.Jobs
{
    public class CommonRegistry : Registry
    {
        internal CommonRegistry()
        {
            Schedule<CoolJob>().ToRunNow().AndEvery(Setting.Value.RefreshInterval).Seconds();
            Schedule<QuasarJob>().ToRunNow().AndEvery(Setting.Value.RefreshInterval).Seconds();
        }
    }
}