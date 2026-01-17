namespace Albatross.Collections.Intervals {
	/// <summary>
	/// Provides fluent comparison extension methods for <see cref="IComparable{T}"/> types.
	/// </summary>
	public static class ComparableExtensions {
		/// <summary>
		/// Determines whether this value is strictly greater than another value.
		/// </summary>
		public static bool IsGreaterThan<T>(this T value, T other) where T : IComparable<T> {
			return value.CompareTo(other) > 0;
		}

		/// <summary>
		/// Determines whether this value is strictly less than another value.
		/// </summary>
		public static bool IsLessThan<T>(this T value, T other) where T : IComparable<T> {
			return value.CompareTo(other) < 0;
		}

		/// <summary>
		/// Determines whether this value is greater than or equal to another value.
		/// </summary>
		public static bool IsGreaterThanOrEqualTo<T>(this T value, T other) where T : IComparable<T> {
			return value.CompareTo(other) >= 0;
		}

		/// <summary>
		/// Determines whether this value is less than or equal to another value.
		/// </summary>
		public static bool IsLessThanOrEqualTo<T>(this T value, T other) where T : IComparable<T> {
			return value.CompareTo(other) <= 0;
		}

		/// <summary>
		/// Determines whether this value falls within a closed interval [start, end], inclusive on both ends.
		/// </summary>
		public static bool IsBetweenInclusive<T>(this T value, T startInclusive, T endInclusive) where T : IComparable<T> {
			return value.IsGreaterThanOrEqualTo(startInclusive) && value.IsLessThanOrEqualTo(endInclusive);
		}

		/// <summary>
		/// Determines whether this value falls within an open interval (start, end), exclusive on both ends.
		/// </summary>
		public static bool IsBetweenExclusive<T>(this T value, T startExclusive, T endExclusive) where T : IComparable<T> {
			return value.IsGreaterThan(startExclusive) && value.IsLessThan(endExclusive);
		}

		/// <summary>
		/// Determines whether this value falls within a half-open interval [start, end), inclusive at start and exclusive at end.
		/// </summary>
		public static bool IsBetween_OpenClosed<T>(this T value, T startInclusive, T endExclusive) where T : IComparable<T> {
			return value.IsGreaterThanOrEqualTo(startInclusive) && value.IsLessThan(endExclusive);
		}

		/// <summary>
		/// Determines whether this value falls within a half-open interval (start, end], exclusive at start and inclusive at end.
		/// </summary>
		public static bool IsBetween_ClosedOpen<T>(this T value, T startExclusive, T endInclusive) where T : IComparable<T> {
			return value.IsGreaterThan(startExclusive) && value.IsLessThanOrEqualTo(endInclusive);
		}
	}
}
