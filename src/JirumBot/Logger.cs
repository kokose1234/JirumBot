using NLog;

namespace JirumBot
{
    public class Logger
    {
        private readonly NLog.Logger logger = LogManager.GetLogger("general_logger");
        private readonly NLog.Logger commandLogger = LogManager.GetLogger("command_logger");
        private readonly NLog.Logger errorLogger = LogManager.GetLogger("exceiption_logger");

        public Logger() => LogManager.LoadConfiguration("NLog.config");

        public NLog.Logger GetLogger() => logger;

        public NLog.Logger GetCommandLogger() => commandLogger;

        public NLog.Logger GetExceptionLogger() => errorLogger;
    }
}