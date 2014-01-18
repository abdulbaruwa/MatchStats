using ReactiveUI;

namespace MatchStats.Logging
{
    public class MatchStatsLogger : ILogger
    {
        private readonly MatchStatsEventSource _inner;


        public MatchStatsLogger(MatchStatsEventSource inner = null)
        {
            _inner = inner ?? RxApp.DependencyResolver.GetService<MatchStatsEventSource>();
        }

        public void Write(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    _inner.Debug(message);
                    break;
                case LogLevel.Error:
                    _inner.Error(message);
                    break;
                case LogLevel.Fatal:
                    _inner.Critical(message);
                    break;
                case LogLevel.Info:
                    _inner.Info(message);
                    break;
                case LogLevel.Warn:
                    _inner.Warn(message);
                    break;
            }
        }

        public LogLevel Level { get; set; }
    }
}