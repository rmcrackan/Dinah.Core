using System;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Dinah.Core.Logging
{
	public static class LoggerExtensions
	{
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
	}
}
