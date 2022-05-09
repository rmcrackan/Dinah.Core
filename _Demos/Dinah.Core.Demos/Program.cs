using System;
using System.Threading;
using System.Threading.Tasks;
using Dinah.Core.ConsoleLib;

namespace Dinah.Core.Demos
{
	public class _Main
	{
		public static async Task Main(string[] args)
		{
			var demo = new Demonstrations();

			//demo.ProgressBarUp();
			//demo.ProgressBarDown();
			demo.ReadPassword();

			//demo.InterruptableTimer_Blocking();
			//demo.InterruptableTimer_Async();
		}
	}
	public class Demonstrations
	{
		public void ProgressBarUp()
		{
			Console.Write("Performing some task... ");

			using var progressBar = new ProgressBar();
			for (int i = 0; i <= 100; i++)
			{
				progressBar.Report((double)i / 100);
				Thread.Sleep(20);
			}

			Console.WriteLine("Done.");
		}

		public void ProgressBarDown()
		{
			Console.Write("Performing some task... ");

			using var progressBar = new ProgressBar();
			for (int i = 100; i >= 0; i--)
			{
				progressBar.Report((double)i / 100);
				Thread.Sleep(20);
			}

			Console.WriteLine("Done.");
		}

		public void ReadPassword()
		{
			Console.Write("Type pw:");
			var pw = ConsoleExt.ReadPassword();
			Console.WriteLine();
			Console.WriteLine("pw=" + pw);
		}

		public void InterruptableTimer_Blocking()
		{
			/*
blocking/sequential: 3 sec, 5 sec, 3 sec 5 sec

0: 5/9/2022 12:12:51 PM
			+1 sec: timer will not start until PerformNow
1: 5/9/2022 12:12:52 PM
START: 5/9/2022 12:12:52 PM  <--+
			+3 sec              |
END: 5/9/2022 12:12:55 PM       +-- 8 sec
			+5 sec              |
START: 5/9/2022 12:13:00 PM  <--+
			+3 sec
END: 5/9/2022 12:13:03 PM
			+5 sec
START: 5/9/2022 12:13:08 PM
			 */
			Console.WriteLine("0: " + DateTime.Now);
			var t = new InterruptableTimer(5_000);

			// WILL block timer
			t.Elapsed += (_, __) =>
			{
				Console.WriteLine("START: " + DateTime.Now);
				// if async, it will NOT BLOCK TIMER
				Task.Delay(3_000).GetAwaiter().GetResult();
				Console.WriteLine("END: " + DateTime.Now);
			};

			Thread.Sleep(1_000);
			Console.WriteLine("1: " + DateTime.Now);
			t.PerformNow();

			Console.ReadLine();
		}
		public void InterruptableTimer_Async()
		{
			/*
non-blocking/async: 3 sec, 2 sec, 3 sec 2 sec

0: 5/9/2022 12:51:36 PM
1: 5/9/2022 12:51:37 PM
START: 5/9/2022 12:51:37 PM  <--+
			+3 sec              |
END: 5/9/2022 12:51:40 PM       +-- 5 sec
			+2 sec (5-3)        |
START: 5/9/2022 12:51:42 PM  <--+
END: 5/9/2022 12:51:45 PM
			 */
			Console.WriteLine("0: " + DateTime.Now);
			var t = new InterruptableTimer(5_000);

			// will NOT block timer
			t.Elapsed += async (_, __) =>
			{
				Console.WriteLine("START: " + DateTime.Now);
				// if async, it will NOT BLOCK TIMER
				await Task.Delay(3_000);
				Console.WriteLine("END: " + DateTime.Now);
			};

			// intentionally using Thread.Sleep instead of Task.Delay to show async timer calls from a non-async method
			Thread.Sleep(1_000);
			Console.WriteLine("1: " + DateTime.Now);
			t.PerformNow();

			Console.ReadLine();
		}
	}
}
