namespace Albatross.Collections.Intervals {
	public record class DateInterval : IClosedInterval<DateOnly> {
		public DateOnly StartInclusive { get; set; }
		public DateOnly EndInclusive { get; set; }
		public static DateOnly Next(DateOnly value) {
			if (value == DateOnly.MaxValue) { throw new ArgumentOutOfRangeException("Max value reached"); }
			return value.AddDays(1);
		}
		public static DateOnly Previous(DateOnly value) {
			if (value == DateOnly.MinValue) { throw new ArgumentOutOfRangeException("Min value reached"); }
			return value.AddDays(-1);
		}
	}

	public record class DateInterval<V> : DateInterval, IClosedInterval<DateOnly, V> where V : IEquatable<V> {
		public DateInterval(DateOnly start, DateOnly end, V value) {
			Value = value;
			this.StartInclusive = start;
			this.EndInclusive = end;
		}
		public V Value { get; set; }
	}

	public static class DateIntervalExtensions {
		public static IEnumerable<DateInterval<V>> Insert<V>(this IEnumerable<DateInterval<V>> series, DateInterval<V> src, Func<V, V> clone) where V : IEquatable<V>
			=> series.Insert<DateInterval<V>, DateOnly>(src,
				(a, b) => EqualityComparer<V>.Default.Equals(a.Value, b.Value),
				a => new DateInterval<V>(a.StartInclusive, a.EndInclusive, clone(a.Value)));

		public static void Update<V>(this ICollection<DateInterval<V>> series, Action<DateInterval<V>> modify, DateOnly start, DateOnly end, Func<V, V> clone) where V : IEquatable<V>
			=> series.Update<DateInterval<V>, DateOnly>(modify, a => new DateInterval<V>(a.StartInclusive, a.EndInclusive, clone(a.Value)), start, end);
	}
}
