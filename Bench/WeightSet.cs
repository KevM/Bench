using System;
using System.Collections.Generic;
using System.Linq;

namespace Bench
{
	/// <summary>
	/// This is a stupidly named helper class. Get it WeightSet?!
	/// </summary>
	public static class WeightSet
	{
		public static void Each<T>(this IEnumerable<T> things, Action<T> action)
		{
			foreach (var thing in things)
			{
				action(thing);
			}
		}

		public static void Times(this int times, Action action)
		{
			for (var i = 0; i < times; i++)
			{
				action();
			}
		}

		public static IEnumerable<T> Times<T>(this int times, Func<T> func)
		{
			return Enumerable.Range(0, times).Select(i => func());
		}

		public static Action DoNothing = () => { };

		public static IEnumerable<TimeSpan> RandomTimings(int count = 100, int maximum = 10000, int minimum = 1)
		{
			var r = new Random();

			return count.Times(()=>
			{
				var ticks = r.Next(minimum, maximum);
				return TimeSpan.FromTicks(ticks);
			});
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng = null)
		{
			rng = rng ?? new Random();

			T[] elements = source.ToArray();
			for (int i = elements.Length - 1; i >= 0; i--)
			{
				// Swap element "i" with a random earlier element it (or itself)
				// ... except we don't really need to swap it fully, as we can
				// return it immediately, and afterwards it's irrelevant.
				int swapIndex = rng.Next(i + 1);
				yield return elements[swapIndex];
				elements[swapIndex] = elements[i];
			}
		}
	}
}