using System;

namespace Bench
{
	public interface IStopWatch
	{
		TimeSpan Elapsed { get; }
		void Start();
		void Stop();
		void Reset();
	}

	public class StopWatch : IStopWatch
	{
		public TimeSpan Elapsed { get; set; }
		private readonly System.Diagnostics.Stopwatch _watch;

		public StopWatch()
		{
			_watch = new System.Diagnostics.Stopwatch();
		}

		public void Start()
		{
			_watch.Start();
		}

		public void Stop()
		{
			_watch.Stop();
		}

		public void Reset()
		{
			_watch.Reset();
		}
	}
}