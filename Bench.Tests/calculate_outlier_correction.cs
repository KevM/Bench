using System;
using System.Linq;
using NUnit.Framework;
using Should;

namespace Bench.Tests
{
	[TestFixture]
	public class calculate_outlier_correction
	{
		[Test]
		public void should_subtract_single_top_and_bottom_outliers()
		{
			var run = TimeSpan.FromMinutes(1);
			var timings = new [] {TimeSpan.FromSeconds(1)}
				.Concat(WeightSet.RandomTimings(minimum:10, maximum:(int)TimeSpan.TicksPerSecond-10))
				.Concat(new [] {TimeSpan.FromTicks(1)})
				.Shuffle();
				
			var result = Bench.CalculateOutlierCorrection(run, timings, 1);

			result.Ticks.ShouldEqual(TimeSpan.TicksPerSecond + 1);
		}

		[Test]
		public void should_subtract_two_top_and_bottom_outliers()
		{
			var run = TimeSpan.FromMinutes(1);
			var timings = new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1) }
				.Concat(WeightSet.RandomTimings(minimum: 10, maximum: (int)TimeSpan.TicksPerSecond - 10))
				.Concat(new[] { TimeSpan.FromTicks(1), TimeSpan.FromTicks(2) })
				.Shuffle();

			var result = Bench.CalculateOutlierCorrection(run, timings, 2);

			result.Ticks.ShouldEqual((TimeSpan.TicksPerSecond*3) + 3);
		}

	}
}