namespace Albatross.Collections.Intervals {
	public record class DateInterval : IClosedInterval<DateOnly> {
		public DateOnly StartInclusive { get; set; }
		public DateOnly EndInclusive { get; set; }
		public static DateOnly Next(DateOnly value) => value.AddDays(1);
		public static DateOnly Previous(DateOnly value) => value.AddDays(-1);
	}

	public record class DateInterval<V> : DateInterval, IClosedInterval<DateOnly, V> where V : ICloneable, IEquatable<V> {
		public V? Value {get;set; }
	}
}
