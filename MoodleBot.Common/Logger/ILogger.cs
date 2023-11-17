using System;

namespace MoodleBot.Common
{
    public interface ILogger
    {
        public void Trace(string message, string userName = null, long? userId = null);
        public void Debug(string message, string userName = null, long? userId = null);
        public void Info(string message, string userName = null, long? userId = null, Exception exception = null);
        public void Warn(string message, Exception exception = null, string userName = null, long? userId = null);
        public void Error(string message, Exception exception, string userName = null, long? userId = null);
        public void Fatal(string message, Exception exception, string userName = null, long? userId = null);
    }
}
