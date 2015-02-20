using System;
using System.Globalization;
using NUnit.Framework;
using Should;

namespace Bench.Tests
{
	public class StubWatch : IStopWatch
	{
		public TimeSpan Elapsed { get; private set; }

		public bool IsStarted { get; set; }
		public bool WasStarted { get; set; }
		public bool WasReset{ get; set; }

		public void SetElapsed(TimeSpan ts)
		{
			Elapsed = ts;
		}

		public void Start()
		{
			IsStarted = true;
			WasStarted = true;
		}

		public void Stop()
		{
			IsStarted = false;
		}

		public void Reset()
		{
			WasReset = true;
		}
	}

	[TestFixture]
	public class bench_tests
	{
		private Bench _cut;
		private StubWatch _fakeWatch;

		[SetUp]
		public void beforeAll()
		{
			_fakeWatch = new StubWatch();
			_cut = new Bench(_fakeWatch);
		}

		[Test]
		public void should_repeat()
		{
			int runCount = 0;

			_cut.Run(()=> { runCount++; });

			runCount.ShouldEqual(_cut.Iterations+_cut.WarmUps);
		}

		[Test]
		public void should_only_time_iterations()
		{
			int runCount = 0;

			Action action = () =>
			{
				runCount++;

				if (runCount <= _cut.WarmUps)
				{
					_fakeWatch.IsStarted.ShouldBeFalse();
				}
				else
				{
					_fakeWatch.IsStarted.ShouldBeTrue();
				}
			};

			_cut.Run(action);

			runCount.ShouldEqual(_cut.Iterations + _cut.WarmUps);
		}

		[Test]
		public void should_report_watches_elapsed_time()
		{
			var expectedResult = TimeSpan.FromMinutes(5);
			_fakeWatch.SetElapsed(expectedResult);
			_cut.Formatter = span =>
			{
				span.ShouldEqual(expectedResult);
				return "";
			};

			_cut.Run(WeightSet.DoNothing);
		}

		[Test]
		public void should_invoke_before_action_before_run()
		{
			var beforeWasInvoked = false;

			Action before = () => beforeWasInvoked = true;
			Action action = () => beforeWasInvoked.ShouldBeTrue();

			_cut.Run(action, before);
		}

		[Test]
		public void should_invoke_after_action_after_run()
		{
			var afterWasInvoked = false;
			
			Action before = () => afterWasInvoked.ShouldBeFalse();
			Action action = () => afterWasInvoked.ShouldBeFalse();
			Action after = () => afterWasInvoked = true;

			_cut.Run(action, before, after);

			afterWasInvoked.ShouldBeTrue();
		}

		[Test]
		public void should_output_result_with_default_formatter()
		{
			_fakeWatch.SetElapsed(TimeSpan.FromSeconds(1.1));
			_cut.Output = result =>
			{
				result.ShouldEqual("This is a handy output format in 1100ms");
			};

			_cut.Run(WeightSet.DoNothing, messageFormat:"This is a handy output format in {0}ms");
		}

		[Test]
		public void should_output_result_with_given_formatter()
		{
			_fakeWatch.SetElapsed(TimeSpan.FromSeconds(0.9));
			_cut.Formatter = span =>
			{
				return span.Ticks.ToString(CultureInfo.InvariantCulture);
			};
			_cut.Output = result =>
			{
				result.ShouldEqual(String.Format("Message in ticks {0}.", TimeSpan.TicksPerSecond * 0.9));
			};

			_cut.Run(WeightSet.DoNothing, messageFormat: "Message in ticks {0}.");
		}
	}
}