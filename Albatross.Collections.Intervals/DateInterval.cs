namespace Albatross.Collections.Intervals {
	public record class DateInterval : IClosedInterval<DateOnly> {
		public DateOnly StartInclusive { get; set; }
		public DateOnly EndInclusive { get; set; }
		public static DateOnly Next(DateOnly value) => value.AddDays(1);
		public static DateOnly Previous(DateOnly value) => value.AddDays(-1);
	}

	public record class DateInterval<V> : DateInterval, IClosedInterval<DateOnly, V> where V : IEquatable<V> {
		public DateInterval(DateOnly start, DateOnly end, V? value) {
			Value = value;
			this.StartInclusive = start;
			this.EndInclusive = end;
		}
		public V? Value { get; set; }
	}

	public static class DateIntervalExtensions {
		public static IEnumerable<DateInterval<V>> Insert<V>(this IEnumerable<DateInterval<V>> series, DateInterval<V> src) where V : IEquatable<V>
			=> series.Insert<DateInterval<V>, DateOnly>(src, 
				(a, b) => EqualityComparer<V>.Default.Equals(a.Value, b.Value), 
				a => new DateInterval<V>(a.StartInclusive, a.EndInclusive, a.Value));
	}
}
