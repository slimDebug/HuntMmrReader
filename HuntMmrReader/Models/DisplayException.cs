using System;

namespace HuntMmrReader.Models;

internal class DisplayException
{
    private readonly DateTime _creationTime;

    internal DisplayException(Type baseException, string message, string? stackTrace)
    {
        BaseException = baseException;
        Message = message;
        StackTrace = stackTrace ?? string.Empty;
        _creationTime = DateTime.Now;
    }

    public Type BaseException { get; }
    public string Message { get; }
    public string StackTrace { get; }
    public string CreationTime => _creationTime.ToString("G");
}