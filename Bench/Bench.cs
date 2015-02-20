using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bench
{
	public class Bench
	{
		public IStopWatch Watch { get; private set; }
		public const string DefaultMessageFormat = "Action took {0}ms.";
		
		/// <summary>
		/// Format timespan as a string for the Output delegate. Default is to return milliseconds.
		/// </summary>
		public Func<TimeSpan, string> Formatter { get; set; } 

		/// <summary>
		/// Delegate responsible for outputting the result of each run of the benchmark. 
		/// Input is the result of the Formatter.
		/// </summary>
		public Action<string> Output { get; set; }

		/// <summary>
		/// Number of executions of the benchmark action before starting measurement.
		/// </summary>
		public int WarmUps { get; set; }

		/// <summary>
		/// Number of measured executions of the action being benchmarked.
		/// </summary>
		public int Iterations { get; set; }

		/// <summary>
		/// Number of benchmark executions. 
		/// </summary>
		public int Runs { get; set; }

		/// <summary>
		/// The N slowest and fastest times will be removed from the results.
		/// </summary>
		public int OutliersToRemove { get; set; }

		public Bench() : this(new StopWatch()) { }

		public Bench(IStopWatch watch)
		{
			Watch = watch;
			Runs = 1;
			WarmUps = 5;
			Iterations = 100;
			OutliersToRemove = 1;
			Output = message => Debug.WriteLine(message);
			Formatter = timespan => timespan.TotalMilliseconds.ToString();
		}

		public void Run(Action action, 
			Action before = null, 
			Action after = null, 
			string messageFormat = DefaultMessageFormat)
		{
			if (before == null) before = WeightSet.DoNothing;
			if (after == null) after = WeightSet.DoNothing;

			Runs.Times(() =>
			{
				before();

				WarmUps.Times(action);

				var timings = new List<TimeSpan>(Runs);
				var iterationWatch = new Stopwatch();

				Watch.Start();
				Iterations.Times(() =>
				{
					iterationWatch.Start();
					action();
					iterationWatch.Stop();
					timings.Add(iterationWatch.Elapsed);
					timings.Clear();
				});
				Watch.Stop();

				var elapsed = Watch.Elapsed - CalculateOutlierCorrection(Watch.Elapsed, timings, OutliersToRemove);

				var result = Formatter(elapsed);
				var message = String.Format(messageFormat, result);
				Output(message);

				after();
			});
		}

		public static TimeSpan CalculateOutlierCorrection(TimeSpan run, IEnumerable<TimeSpan> timings, int outliersToRemove)
		{
			var sorted = timings.OrderBy(t => t.Ticks).ToArray();
			
			var left = sorted.Take(outliersToRemove);
			var right = sorted.Reverse().Take(outliersToRemove);
			
			var outliers = left.Concat(right).ToArray();

			return TimeSpan.FromTicks(outliers.Sum(o => o.Ticks));
		}
	}
}