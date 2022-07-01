using JetBrains.Annotations;
using NLog;

namespace ItsReactive.Core.Adapters;

public class LoggerAdapter<T> : ILoggerAdapter<T>
{
    private readonly ILogger _logger;

    public LoggerAdapter(ILogger logger)
    {
        _logger = logger;
    }
    
    public void LogInformation([StructuredMessageTemplate]string? message, params object?[] args)
    {
        if (!_logger.IsEnabled(LogLevel.Info)) return;
        
        _logger.Log(LogLevel.Info, message, args: args);
    }

    public void LogDebug([StructuredMessageTemplate]string? message, params object?[] args)
    {
        if (!_logger.IsEnabled(LogLevel.Debug)) return;
        
        _logger.Log(LogLevel.Debug, message, args: args);
    }

    public void LogWarning([StructuredMessageTemplate]string? message, params object?[] args)
    {
        if (!_logger.IsEnabled(LogLevel.Warn)) return;
        
        _logger.Log(LogLevel.Warn, message, args: args);
    }

    public void LogError([StructuredMessageTemplate]string? message, params object?[] args)
    {
        if (!_logger.IsEnabled(LogLevel.Error)) return;
        Console.WriteLine(_logger.IsEnabled(LogLevel.Error));
        _logger.Log(LogLevel.Error, message, args: args);
    }
}