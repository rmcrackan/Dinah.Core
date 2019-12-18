using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Dinah.Core.Logging
{
	// from
	// https://gist.github.com/nblumhardt/0e1e22f50fe79de60ad257f77653c813

	class CallerEnricher : ILogEventEnricher
	{
		public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
		{
			var skip = 3;
			while (true)
			{
				var stack = new StackFrame(skip);
				if (!stack.HasMethod())
				{
					logEvent.AddPropertyIfAbsent(new LogEventProperty("Caller", new ScalarValue("<unknown method>")));
					return;
				}

				var method = stack.GetMethod();
				if (method.DeclaringType != null && method.DeclaringType.Assembly != typeof(Log).Assembly)
				{
					var caller = $"{method.DeclaringType.FullName}.{method.Name}({string.Join(", ", method.GetParameters().Select(pi => pi.ParameterType.FullName))})";
					logEvent.AddPropertyIfAbsent(new LogEventProperty("Caller", new ScalarValue(caller)));
				}

				skip++;
			}
		}
	}

	public static class LoggerCallerEnrichmentConfiguration
	{
		public static LoggerConfiguration WithCaller(this LoggerEnrichmentConfiguration enrichmentConfiguration)
		{
			return enrichmentConfiguration.With<CallerEnricher>();
		}
	}

	class ExampleProgram
	{
		static void __example()
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.WithCaller()
				.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message} (at {Caller}){NewLine}{Exception}")
				.CreateLogger();

			Log.Information("Hello, world!");

			SayGoodbye();

			Log.CloseAndFlush();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static void SayGoodbye()
		{
			Log.Information("Goodbye!");
		}
	}
}
