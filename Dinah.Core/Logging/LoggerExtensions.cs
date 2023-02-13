using System;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Dinah.Core.Logging
{
	public static class LoggerExtensions
	{
		public static bool IsVerboseEnabled(this ILogger logger) => logger.IsEnabled(LogEventLevel.Verbose);
		public static bool IsDebugEnabled(this ILogger logger) => logger.IsEnabled(LogEventLevel.Debug);
		public static bool IsInformationEnabled(this ILogger logger) => logger.IsEnabled(LogEventLevel.Information);
		public static bool IsWarningEnabled(this ILogger logger) => logger.IsEnabled(LogEventLevel.Warning);
		public static bool IsErrorEnabled(this ILogger logger) => logger.IsEnabled(LogEventLevel.Error);
		public static bool IsFatalEnabled(this ILogger logger) => logger.IsEnabled(LogEventLevel.Fatal);

		public static ILogger Here(this ILogger logger,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0)
		{
			return logger
				.ForContext("MemberName", memberName)
				.ForContext("FilePath", sourceFilePath)
				.ForContext("LineNumber", sourceLineNumber);
		}
		static void __example()
		{
			var outputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";

			Log.Logger = new LoggerConfiguration()
						.MinimumLevel.Warning()
						.Enrich.FromLogContext()
						.WriteTo.Console(LogEventLevel.Warning, outputTemplate, theme: AnsiConsoleTheme.Literate)
						.CreateLogger();

			Log.Logger.Here().Information("Hello, world!");
		}

		#region TryLog

		#region TryLogVerbose
		public static bool TryLogVerbose(this ILogger logger, string messageTemplate)
			=> tryLog(() => logger.Verbose(messageTemplate));
		public static bool TryLogVerbose<T>(this ILogger logger, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Verbose(messageTemplate, propertyValue));
		public static bool TryLogVerbose<T0, T1>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Verbose(messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogVerbose<T0, T1, T2>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Verbose(messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogVerbose(this ILogger logger, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Verbose(messageTemplate, propertyValues));
		public static bool TryLogVerbose(this ILogger logger, Exception exception, string messageTemplate)
			=> tryLog(() => logger.Verbose(exception, messageTemplate));
		public static bool TryLogVerbose<T>(this ILogger logger, Exception exception, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Verbose(exception, messageTemplate, propertyValue));
		public static bool TryLogVerbose<T0, T1>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Verbose(exception, messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogVerbose<T0, T1, T2>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Verbose(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogVerbose(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Verbose(exception, messageTemplate, propertyValues));
		#endregion
		#region TryLogDebug
		public static bool TryLogDebug(this ILogger logger, string messageTemplate)
			=> tryLog(() => logger.Debug(messageTemplate));
		public static bool TryLogDebug<T>(this ILogger logger, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Debug(messageTemplate, propertyValue));
		public static bool TryLogDebug<T0, T1>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Debug(messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogDebug<T0, T1, T2>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Debug(messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogDebug(this ILogger logger, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Debug(messageTemplate, propertyValues));
		public static bool TryLogDebug(this ILogger logger, Exception exception, string messageTemplate)
			=> tryLog(() => logger.Debug(exception, messageTemplate));
		public static bool TryLogDebug<T>(this ILogger logger, Exception exception, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Debug(exception, messageTemplate, propertyValue));
		public static bool TryLogDebug<T0, T1>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Debug(exception, messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogDebug<T0, T1, T2>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Debug(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogDebug(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Debug(exception, messageTemplate, propertyValues));
		#endregion
		#region TryLogInformation
		public static bool TryLogInformation(this ILogger logger, string messageTemplate)
			=> tryLog(() => logger.Information(messageTemplate));
		public static bool TryLogInformation<T>(this ILogger logger, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Information(messageTemplate, propertyValue));
		public static bool TryLogInformation<T0, T1>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Information(messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogInformation<T0, T1, T2>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Information(messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogInformation(this ILogger logger, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Information(messageTemplate, propertyValues));
		public static bool TryLogInformation(this ILogger logger, Exception exception, string messageTemplate)
			=> tryLog(() => logger.Information(exception, messageTemplate));
		public static bool TryLogInformation<T>(this ILogger logger, Exception exception, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Information(exception, messageTemplate, propertyValue));
		public static bool TryLogInformation<T0, T1>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Information(exception, messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogInformation<T0, T1, T2>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Information(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogInformation(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Information(exception, messageTemplate, propertyValues));
		#endregion
		#region TryLogWarning
		public static bool TryLogWarning(this ILogger logger, string messageTemplate)
			=> tryLog(() => logger.Warning(messageTemplate));
		public static bool TryLogWarning<T>(this ILogger logger, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Warning(messageTemplate, propertyValue));
		public static bool TryLogWarning<T0, T1>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Warning(messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogWarning<T0, T1, T2>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Warning(messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogWarning(this ILogger logger, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Warning(messageTemplate, propertyValues));
		public static bool TryLogWarning(this ILogger logger, Exception exception, string messageTemplate)
			=> tryLog(() => logger.Warning(exception, messageTemplate));
		public static bool TryLogWarning<T>(this ILogger logger, Exception exception, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Warning(exception, messageTemplate, propertyValue));
		public static bool TryLogWarning<T0, T1>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Warning(exception, messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogWarning<T0, T1, T2>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Warning(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogWarning(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Warning(exception, messageTemplate, propertyValues));
		#endregion
		#region TryLogError
		public static bool TryLogError(this ILogger logger, string messageTemplate)
			=> tryLog(() => logger.Error(messageTemplate));
		public static bool TryLogError<T>(this ILogger logger, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Error(messageTemplate, propertyValue));
		public static bool TryLogError<T0, T1>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Error(messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogError<T0, T1, T2>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Error(messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogError(this ILogger logger, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Error(messageTemplate, propertyValues));
		public static bool TryLogError(this ILogger logger, Exception exception, string messageTemplate)
			=> tryLog(() => logger.Error(exception, messageTemplate));
		public static bool TryLogError<T>(this ILogger logger, Exception exception, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Error(exception, messageTemplate, propertyValue));
		public static bool TryLogError<T0, T1>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Error(exception, messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogError<T0, T1, T2>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Error(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogError(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Error(exception, messageTemplate, propertyValues));
		#endregion
		#region TryLogFatal
		public static bool TryLogFatal(this ILogger logger, string messageTemplate)
			=> tryLog(() => logger.Fatal(messageTemplate));
		public static bool TryLogFatal<T>(this ILogger logger, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Fatal(messageTemplate, propertyValue));
		public static bool TryLogFatal<T0, T1>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Fatal(messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogFatal<T0, T1, T2>(this ILogger logger, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Fatal(messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogFatal(this ILogger logger, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Fatal(messageTemplate, propertyValues));
		public static bool TryLogFatal(this ILogger logger, Exception exception, string messageTemplate)
			=> tryLog(() => logger.Fatal(exception, messageTemplate));
		public static bool TryLogFatal<T>(this ILogger logger, Exception exception, string messageTemplate, T propertyValue)
			=> tryLog(() => logger.Fatal(exception, messageTemplate, propertyValue));
		public static bool TryLogFatal<T0, T1>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
			=> tryLog(() => logger.Fatal(exception, messageTemplate, propertyValue0, propertyValue1));
		public static bool TryLogFatal<T0, T1, T2>(this ILogger logger, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
			=> tryLog(() => logger.Fatal(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2));
		public static bool TryLogFatal(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues)
			=> tryLog(() => logger.Fatal(exception, messageTemplate, propertyValues));
		#endregion

		private static bool tryLog(Action logAction)
		{
			try
			{
				logAction();
				return true;
			}
			catch { return false; }
		}
		#endregion
	}
}
