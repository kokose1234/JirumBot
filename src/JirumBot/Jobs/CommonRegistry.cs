using FluentScheduler;
using JirumBot.Data;
using System;

namespace JirumBot.Jobs
{
    public class CommonRegistry : Registry
    {
        internal CommonRegistry()
        {
            var now = DateTime.Now;

            if (now.Minute != 0)
            {
                now = now.AddSeconds(now.Second * -1);
                now = now.AddMinutes(60 - now.Minute);
            }

            Schedule<PpomJob>().ToRunNow().AndEvery(Setting.Value.RefreshInterval).Seconds();
            Schedule<CoolJob>().ToRunNow().AndEvery(Setting.Value.RefreshInterval).Seconds();
            Schedule<QuasarJob>().ToRunNow().AndEvery(Setting.Value.RefreshInterval).Seconds();
            Schedule<FmJob>().ToRunNow().AndEvery(Setting.Value.RefreshInterval).Seconds();
            Schedule<ClienJob>().ToRunNow().AndEvery(Setting.Value.RefreshInterval).Seconds();
            Schedule<RuliJob>().ToRunNow().AndEvery(Setting.Value.RefreshInterval).Seconds();
            Schedule<MeecoJob>().ToRunNow().AndEvery(Setting.Value.RefreshInterval).Seconds();

            Schedule<GhostWatcher>().ToRunOnceAt(DateTime.Now.AddSeconds(5)).AndEvery(Setting.Value.GhostCheckInterval).Seconds();
        }
    }
}