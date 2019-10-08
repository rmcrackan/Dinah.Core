using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

// from: https://blogs.msdn.microsoft.com/dbrowne/2017/09/22/simple-logging-for-ef-core/
/*
USAGE
=====
// Once you configure logging on a DbContext instance it will be enabled on all instances of that DbContext type. Repeated calls to ConfigureLogging() will change the logging for the DbContext type. Behind the scenes there is a single LogProvider for each DbContext type.

using var db = new BloggingContext();
db.ConfigureLogging(s => Console.WriteLine(s));
// ...

You can also filter the logger by LogLevel and/or category with an optional second parameter. Here the logger will capture all Errors and all Queries like this:
using var db = new BloggingContext();
db.ConfigureLogging(s => Console.WriteLine(s), (c, l) => l == LogLevel.Error || c == DbLoggerCategory.Query.Name);
// ...

Shortcut to log the Query, Command, and Update categories to capture the all the generated SQL and execution statistics
var db = new BloggingContext();
db.ConfigureLogging(s => Console.WriteLine(s), LoggingCategories.SQL);
// ...
*/
namespace Dinah.EntityFrameworkCore
{
    public enum LoggingCategories
    {
        All = 0,
        SQL = 1
    }

    public static class DbContextLoggingExtensions
    {
        static string[] sqlCategories { get; } = new[] { DbLoggerCategory.Database.Command.Name, DbLoggerCategory.Query.Name, DbLoggerCategory.Update.Name };

        public static void ConfigureLogging(this DbContext db, Action<string> logger, LoggingCategories categories = LoggingCategories.All)
        {
            Func<string, LogLevel, bool> filter
                = categories == LoggingCategories.SQL ? (c, l) => sqlCategories.Contains(c)
                : categories == LoggingCategories.All ? (Func<string, LogLevel, bool>)((_, __) => true)
                : throw new ArgumentException("bad logging category", nameof(categories));
            db.ConfigureLogging(logger, filter);
        }
        public static void ConfigureLogging(this DbContext db, Action<string> logger, Func<string, LogLevel, bool> filter)
        {
            var serviceProvider = db.GetInfrastructure();
            var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            LogProvider.CreateOrModifyLoggerForDbContext(db.GetType(), loggerFactory, logger, filter);
        }
    }
    class LogProvider : ILoggerProvider
    {
        // volatile to allow the configuration to be switched without locking
        public volatile LoggingConfiguration Configuration;
        static bool DefaultFilter(string CategoryName, LogLevel level) => true;

        static ConcurrentDictionary<Type, LogProvider> providers = new ConcurrentDictionary<Type, LogProvider>();

        public static void CreateOrModifyLoggerForDbContext(Type DbContextType, ILoggerFactory loggerFactory, Action<string> logger,
            Func<string, LogLevel, bool> filter = null)
        {
            bool isNew = false;
            var provider = providers.GetOrAdd(DbContextType, t =>
                {
                    var p = new LogProvider(logger, filter ?? DefaultFilter);
                    loggerFactory.AddProvider(p);
                    isNew = true;
                    return p;
                });
            if (!isNew)
                provider.Configuration = new LoggingConfiguration(logger, filter ?? DefaultFilter);
        }

        public class LoggingConfiguration
        {
            public Action<string> logger { get; }
            public Func<string, LogLevel, bool> filter { get; }
            public LoggingConfiguration(Action<string> logger, Func<string, LogLevel, bool> filter)
            {
                this.logger = logger;
                this.filter = filter;
            }
        }

        private LogProvider(Action<string> logger, Func<string, LogLevel, bool> filter) => this.Configuration = new LoggingConfiguration(logger, filter);

        public ILogger CreateLogger(string categoryName) => new Logger(categoryName, this);

        public void Dispose() { }

        private class Logger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel) => true;

            string categoryName { get; }
            LogProvider provider { get; }

            public Logger(string categoryName, LogProvider provider)
            {
                this.provider = provider;
                this.categoryName = categoryName;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                // grab a reference to the current logger settings for consistency, and to eliminate the need to block a thread reconfiguring the logger
                var config = provider.Configuration;
                if (config.filter(categoryName, logLevel))
                    config.logger(formatter(state, exception));
            }

            public IDisposable BeginScope<TState>(TState state) => null;
        }
    }
}