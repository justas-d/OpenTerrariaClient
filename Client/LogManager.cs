using System;

namespace OpenTerrariaClient.Client
{
    public sealed class ClassLogMessageEventArgs : EventArgs
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

        public ClassLogMessageEventArgs(string message, LogSeverity severity)
        {
            Message = message;
            Severity = severity;
            Time = DateTime.Now;
        }
    }

    public class LogManager
    {
        public event EventHandler<ClassLogMessageEventArgs> MessageReceived = delegate { };

        internal LogManager() { }

        private void OnMessageReceived(string message, ClassLogMessageEventArgs.LogSeverity severity)
            => MessageReceived(this, new ClassLogMessageEventArgs(message, severity));

        public void Info(string message)
            => OnMessageReceived(message, ClassLogMessageEventArgs.LogSeverity.Info);

        public void Warning(string message)
           => OnMessageReceived(message, ClassLogMessageEventArgs.LogSeverity.Warning);

        public void Critical(string message)
           => OnMessageReceived(message, ClassLogMessageEventArgs.LogSeverity.Critical);
    }
}
