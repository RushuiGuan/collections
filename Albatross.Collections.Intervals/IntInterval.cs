namespace Albatross.Collections.Intervals {
	/// <summary>
	/// A closed interval implementation using <see cref="int"/> as the key type. Each unit represents one integer step.
	/// </summary>
	public record class IntInterval : IClosedInterval<int> {
		public int StartInclusive { get; set; }
		public int EndInclusive { get; set; }

		/// <summary>
		/// Returns the next integer after the given value.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is <see cref="int.MaxValue"/>.</exception>
		public static int Next(int value) {
			if (value == int.MaxValue) { throw new ArgumentOutOfRangeException("Max value reached"); }
			return value + 1;
		}

		/// <summary>
		/// Returns the previous integer before the given value.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is <see cref="int.MinValue"/>.</exception>
		public static int Previous(int value) {
			if (value == int.MinValue) { throw new ArgumentOutOfRangeException("Min value reached"); }
			return value - 1;
		}

	}

	/// <summary>
	/// A closed integer interval with an associated value. Useful for numeric ranges that map to values.
	/// </summary>
	/// <typeparam name="V">The type of value associated with the integer range.</typeparam>
	public record class IntInterval<V> : IntInterval, IClosedInterval<int, V> where V : IEquatable<V> {
		public IntInterval(int start, int end, V value) {
			Value = value;
			this.StartInclusive = start;
			this.EndInclusive = end;
		}
		public V Value { get; set; }
	}

	/// <summary>
	/// Provides convenience extension methods for <see cref="IntInterval{V}"/> collections.
	/// </summary>
	public static class IntIntervalExtensions {
		/// <summary>
		/// Inserts an integer interval into a series, maintaining continuity and merging adjacent intervals with equal values.
		/// </summary>
		/// <typeparam name="V">The type of value associated with intervals.</typeparam>
		/// <param name="series">The existing interval series.</param>
		/// <param name="src">The interval to insert.</param>
		/// <param name="clone">Function to clone the value when splitting intervals.</param>
		/// <returns>The updated interval series.</returns>
		public static IEnumerable<IntInterval<V>> Insert<V>(this IEnumerable<IntInterval<V>> series, IntInterval<V> src, Func<V, V> clone) where V : IEquatable<V>
			=> series.Insert<IntInterval<V>, int>(src,
				(a, b) => EqualityComparer<V>.Default.Equals(a.Value, b.Value),
				a => new IntInterval<V>(a.StartInclusive, a.EndInclusive, clone(a.Value)));

		/// <summary>
		/// Updates values within an integer range, automatically splitting intervals as needed.
		/// </summary>
		/// <typeparam name="V">The type of value associated with intervals.</typeparam>
		/// <param name="series">The interval series to update.</param>
		/// <param name="modify">Action to apply to intervals within the range.</param>
		/// <param name="start">The start of the update range (inclusive).</param>
		/// <param name="end">The end of the update range (inclusive).</param>
		/// <param name="clone">Function to clone the value when splitting intervals.</param>
		public static void Update<V>(this ICollection<IntInterval<V>> series, Action<IntInterval<V>> modify, int start, int end, Func<V, V> clone) where V : IEquatable<V>
			=> series.Update<IntInterval<V>, int>(modify, a => new IntInterval<V>(a.StartInclusive, a.EndInclusive, clone(a.Value)), start, end);
	}
}
