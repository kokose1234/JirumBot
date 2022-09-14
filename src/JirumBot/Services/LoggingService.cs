using Discord;
using Discord.WebSocket;
using NLog;

namespace JirumBot.Services;

public class LoggingService
{
    private readonly DiscordSocketClient _discord;
    private readonly Logger _logger = LogManager.GetLogger("general_logger");
    private readonly Logger _commandLogger = LogManager.GetLogger("command_logger");
    private readonly Logger _errorLogger = LogManager.GetLogger("exceiption_logger");

    public LoggingService(DiscordSocketClient discord)
    {
        _discord = discord;
        _discord.Log += DiscordOnLog;

        LogManager.LoadConfiguration("NLog.config");
    }

    private Task DiscordOnLog(LogMessage msg)
    {
        _logger.Info(msg.ToString());
        return Task.CompletedTask;
    }

    public Logger GetLogger() => _logger;

    public Logger GetCommandLogger() => _commandLogger;

    public Logger GetExceptionLogger() => _errorLogger;
}