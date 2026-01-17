namespace Albatross.Collections.Intervals {
	/// <summary>
	/// Provides extension methods for <see cref="IOpenClosedInterval{T}"/>.
	/// </summary>
	public static class OpenClosedIntervalExtensions {
		/// <summary>
		/// Determines whether the interval is valid (start &lt; end).
		/// </summary>
		/// <typeparam name="T">The type of the interval boundaries.</typeparam>
		/// <param name="interval">The interval to validate.</param>
		/// <returns>True if the start is strictly less than the end; otherwise, false.</returns>
		public static bool IsValid<T>(this IOpenClosedInterval<T> interval) where T:IComparable<T>{
			// StartInclusive < EndExclusive
			return interval.StartInclusive.CompareTo(interval.EndExclusive) < 0;
		}
	}
}
