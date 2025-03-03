namespace Albatross.Collections.Intervals {
	public record class DateInterval : IClosedInterval<DateOnly> {
		public DateOnly StartInclusive { get; set; }
		public DateOnly EndInclusive { get; set; }
		public DateOnly Next(DateOnly value) => value.AddDays(1);
		public DateOnly Previous(DateOnly value) => value.AddDays(-1);
	}
}
