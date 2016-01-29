using System;

namespace TerrariaBridge.Client
{
    public sealed class classLogMessageEventArgs : EventArgs
    {
        public enum LogSeverity
        {
            Info,
            Warning,
            Critical,
        }

        public string Message { get; }
        public DateTime Time { get; }
        public LogSeverity Severity { get; }

        public classLogMessageEventArgs(string message, LogSeverity severity)
        {
            Message = message;
            Severity = severity;
            Time = DateTime.Now;
        }
    }

    public class Logger
    {
        public event EventHandler<classLogMessageEventArgs> MessageReceived = delegate { };

        internal Logger() { }

        private void OnMessageReceived(string message, classLogMessageEventArgs.LogSeverity severity)
            => MessageReceived(this, new classLogMessageEventArgs(message, severity));

        public void Info(string message)
            => OnMessageReceived(message, classLogMessageEventArgs.LogSeverity.Info);

        public void Warning(string message)
           => OnMessageReceived(message, classLogMessageEventArgs.LogSeverity.Warning);

        public void Critical(string message)
           => OnMessageReceived(message, classLogMessageEventArgs.LogSeverity.Critical);
    }
}
