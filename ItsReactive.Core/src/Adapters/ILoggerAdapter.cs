﻿namespace ItsReactive.Core.Adapters;

public interface ILoggerAdapter<T>
{
    void LogInformation(string? message, params object?[] args);
    void LogDebug(string? message, params object?[] args);
    void LogWarning(string? message, params object?[] args);
    void LogError(string? message, params object?[] args);
}