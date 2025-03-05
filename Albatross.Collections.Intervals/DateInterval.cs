namespace Albatross.Collections.Intervals {
	public record class DateInterval : IClosedInterval<DateOnly> {
		public DateOnly StartInclusive { get; set; }
		public DateOnly EndInclusive { get; set; }
		public DateOnly Next(DateOnly value) => value.AddDays(1);
		public DateOnly Previous(DateOnly value) => value.AddDays(-1);
	}

	public class DateInterval<V>: ClosedInterval<DateOnly, V> where V : ICloneable, IEquatable<V> {
		public DateInterval(DateOnly startInclusive, DateOnly endInclusive) : base(startInclusive, endInclusive) { }
		public override DateOnly Next(DateOnly value) => value.AddDays(1);
		public override DateOnly Previous(DateOnly value) => value.AddDays(-1);
	}
}
