namespace Albatross.Collections.Intervals {
	/// <summary>
	/// A closed interval implementation using <see cref="DateOnly"/> as the key type. Each unit represents one day.
	/// </summary>
	public record class DateInterval : IClosedInterval<DateOnly> {
		public DateOnly StartInclusive { get; set; }
		public DateOnly EndInclusive { get; set; }

		/// <summary>
		/// Returns the next day after the given date.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is <see cref="DateOnly.MaxValue"/>.</exception>
		public static DateOnly Next(DateOnly value) {
			if (value == DateOnly.MaxValue) { throw new ArgumentOutOfRangeException("Max value reached"); }
			return value.AddDays(1);
		}

		/// <summary>
		/// Returns the previous day before the given date.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is <see cref="DateOnly.MinValue"/>.</exception>
		public static DateOnly Previous(DateOnly value) {
			if (value == DateOnly.MinValue) { throw new ArgumentOutOfRangeException("Min value reached"); }
			return value.AddDays(-1);
		}
	}

	/// <summary>
	/// A closed date interval with an associated value. Useful for time-series data where values change over date ranges.
	/// </summary>
	/// <typeparam name="V">The type of value associated with the date range.</typeparam>
	public record class DateInterval<V> : DateInterval, IClosedInterval<DateOnly, V> where V : IEquatable<V> {
		public DateInterval(DateOnly start, DateOnly end, V value) {
			Value = value;
			this.StartInclusive = start;
			this.EndInclusive = end;
		}
		public V Value { get; set; }
	}

	/// <summary>
	/// Provides convenience extension methods for <see cref="DateInterval{V}"/> collections.
	/// </summary>
	public static class DateIntervalExtensions {
		/// <summary>
		/// Inserts a date interval into a series, maintaining continuity and merging adjacent intervals with equal values.
		/// </summary>
		/// <typeparam name="V">The type of value associated with intervals.</typeparam>
		/// <param name="series">The existing interval series.</param>
		/// <param name="src">The interval to insert.</param>
		/// <param name="clone">Function to clone the value when splitting intervals.</param>
		/// <returns>The updated interval series.</returns>
		public static IEnumerable<DateInterval<V>> Insert<V>(this IEnumerable<DateInterval<V>> series, DateInterval<V> src, Func<V, V> clone) where V : IEquatable<V>
			=> series.Insert<DateInterval<V>, DateOnly>(src,
				(a, b) => EqualityComparer<V>.Default.Equals(a.Value, b.Value),
				a => new DateInterval<V>(a.StartInclusive, a.EndInclusive, clone(a.Value)));

		/// <summary>
		/// Updates values within a date range, automatically splitting intervals as needed.
		/// </summary>
		/// <typeparam name="V">The type of value associated with intervals.</typeparam>
		/// <param name="series">The interval series to update.</param>
		/// <param name="modify">Action to apply to intervals within the range.</param>
		/// <param name="start">The start date of the update range (inclusive).</param>
		/// <param name="end">The end date of the update range (inclusive).</param>
		/// <param name="clone">Function to clone the value when splitting intervals.</param>
		public static void Update<V>(this ICollection<DateInterval<V>> series, Action<DateInterval<V>> modify, DateOnly start, DateOnly end, Func<V, V> clone) where V : IEquatable<V>
			=> series.Update<DateInterval<V>, DateOnly>(modify, a => new DateInterval<V>(a.StartInclusive, a.EndInclusive, clone(a.Value)), start, end);
	}
}
