namespace Albatross.Collections.Intervals {
	public static class OpenClosedIntervalExtensions {
		public static bool IsValid<T>(this IOpenClosedInterval<T> interval) where T:IComparable<T>{
			// StartInclusive < EndExclusive
			return interval.StartInclusive.CompareTo(interval.EndExclusive) < 0;
		}
	}
}
