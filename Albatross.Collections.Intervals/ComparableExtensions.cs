namespace Albatross.Collections.Intervals {
	public static class ComparableExtensions {
		public static bool IsGreaterThan<T>(this T value, T other) where T : IComparable<T> {
			return value.CompareTo(other) > 0;
		}
		public static bool IsLessThan<T>(this T value, T other) where T : IComparable<T> {
			return value.CompareTo(other) < 0;
		}
		public static bool IsGreaterThanOrEqualTo<T>(this T value, T other) where T : IComparable<T> {
			return value.CompareTo(other) >= 0;
		}
		public static bool IsLessThanOrEqualTo<T>(this T value, T other) where T : IComparable<T> {
			return value.CompareTo(other) <= 0;
		}
		public static bool IsBetweenInclusive<T>(this T value, T startInclusive, T endInclusive) where T : IComparable<T> {
			return value.IsGreaterThanOrEqualTo(startInclusive) && value.IsLessThanOrEqualTo(endInclusive);
		}
		public static bool IsBetweenExclusive<T>(this T value, T startExclusive, T endExclusive) where T : IComparable<T> {
			return value.IsGreaterThan(startExclusive) && value.IsLessThan(endExclusive);
		}
		/// <summary>
		/// return true if value is greater than or equal to start and less than end
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="startInclusive"></param>
		/// <param name="endExclusive"></param>
		/// <returns></returns>
		public static bool IsBetween_OpenClosed<T>(this T value, T startInclusive, T endExclusive) where T : IComparable<T> {
			return value.IsGreaterThanOrEqualTo(startInclusive) && value.IsLessThan(endExclusive);
		}
		/// <summary>
		/// return true if value is greater than start and less than or equal to end
		/// </summary>
		public static bool IsBetween_ClosedOpen<T>(this T value, T startExclusive, T endInclusive) where T : IComparable<T> {
			return value.IsGreaterThan(startExclusive) && value.IsLessThanOrEqualTo(endInclusive);
		}
	}
}
