using System;

namespace HashTag.Contracts.Loggers
{
    public interface IApplicationLogger
    {
        void LogInfo(string message);
        void LogError(Exception exception);
        void LogFatal(Exception exception);
        
    }
}