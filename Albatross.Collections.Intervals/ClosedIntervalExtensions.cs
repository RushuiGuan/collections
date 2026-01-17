using System.Diagnostics.CodeAnalysis;

namespace Albatross.Collections.Intervals {
	/// <summary>
	/// Provides extension methods for managing continuous, non-overlapping interval series.
	/// All operations maintain the invariants that intervals are continuous (no gaps) and non-overlapping.
	/// </summary>
	public static class ClosedIntervalExtensions {
		/// <summary>
		/// Determines whether the interval is valid (start &lt;= end).
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <param name="interval">The interval to validate.</param>
		/// <returns>True if start is less than or equal to end; otherwise, false.</returns>
		public static bool IsValid<T>(this IClosedInterval<T> interval) where T : IComparable<T> {
			return interval.StartInclusive.CompareTo(interval.EndInclusive) <= 0;
		}

		/// <summary>
		/// Inserts an interval into a series while maintaining continuity and non-overlap.
		/// Adjacent intervals with equal values (as determined by <paramref name="isEqual"/>) are automatically merged.
		/// The input series doesn't need to be sorted. Time complexity is O(n).
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="series">The existing interval series.</param>
		/// <param name="src">The interval to insert.</param>
		/// <param name="isEqual">Function to determine if two intervals have equal values and should be merged.</param>
		/// <param name="clone">Function to create a copy of an interval when splitting is required.</param>
		/// <returns>The updated interval series.</returns>
		/// <exception cref="ArgumentException">Thrown when the insertion would break series continuity or when start &gt; end.</exception>
		public static IEnumerable<T> Insert<T, K>(this IEnumerable<T> series, T src, Func<T, T, bool> isEqual, Func<T, T> clone)
			where T : IClosedInterval<K> where K : IComparable<K> {
			if (src.StartInclusive.CompareTo(src.EndInclusive) > 0) { throw new ArgumentException("Start date cannot be greater than end date"); }
			bool isContinuous = false;
			bool isEmpty = true;
			foreach (var item in series) {
				isEmpty = false;
				// source completely overlaps item, if same value, extend current and replace source with current.  otherwise do nothing
				if (src.StartInclusive.CompareTo(item.StartInclusive) <= 0 && item.EndInclusive.CompareTo(src.EndInclusive) <= 0) {
					isContinuous = true;
					if (isEqual(src, item)) {
						item.StartInclusive = src.StartInclusive;
						item.EndInclusive = src.EndInclusive;
						src = item;
					}
				} else if (item.StartInclusive.CompareTo(src.StartInclusive) < 0 && src.EndInclusive.CompareTo(item.EndInclusive) < 0) {
					isContinuous = true;
					// item completely overlaps source, if same value, do nothing.  otherwise split the item
					if (isEqual(src, item)) {
						src = item;
					} else {
						var after = clone(item);
						after.StartInclusive = T.Next(src.EndInclusive);
						// note that item.EndInclusive is modified.  therefore after.EndInclusive should be set first.
						after.EndInclusive = item.EndInclusive;
						item.EndInclusive = T.Previous(src.StartInclusive);
						yield return item;
						yield return after;
					}
				} else if (src.StartInclusive.CompareTo(item.StartInclusive) <= 0 && item.StartInclusive.CompareTo(src.EndInclusive) <= 0 && src.EndInclusive.CompareTo(item.EndInclusive) < 0) {
					isContinuous = true;
					// source overlaps the start of item, if same value, extend item.  otherwise, reduce item start date
					if (isEqual(src, item)) {
						item.StartInclusive = src.StartInclusive;
						src = item;
					} else {
						item.StartInclusive = T.Next(src.EndInclusive);
						yield return item;
					}
				} else if (item.StartInclusive.CompareTo(src.StartInclusive) < 0 && src.StartInclusive.CompareTo(item.EndInclusive) <= 0 && item.EndInclusive.CompareTo(src.EndInclusive) <= 0) {
					isContinuous = true;
					if (isEqual(src, item)) {
						item.EndInclusive = src.EndInclusive;
						src = item;
					} else {
						item.EndInclusive = T.Previous(src.StartInclusive);
						yield return item;
					}
				} else if (src.EndInclusive.CompareTo(item.StartInclusive) < 0 && src.EndInclusive.CompareTo(T.Previous(item.StartInclusive)) == 0) {
					// ^ the first condition above is to make sure that item.StartInclusive is not at min value.  If true, the Previous(item.StartInclusive) will throw an exception
					isContinuous = true;
					if (isEqual(src, item)) {
						item.StartInclusive = src.StartInclusive;
						src = item;
					} else {
						yield return item;
					}
				} else if (src.StartInclusive.CompareTo(item.EndInclusive) > 0 && src.StartInclusive.CompareTo(T.Next(item.EndInclusive)) == 0) {
					// ^ the first condition above is to make sure that item.EndInclusive is not at max value.  If true, the Next(item.EndInclusive) will throw an exception
					isContinuous = true;
					if (isEqual(src, item)) {
						item.EndInclusive = src.EndInclusive;
						src = item;
					} else {
						yield return item;
					}
				} else {
					yield return item;
				}
			}
			// yield return first so that the data is as closed to correct as possible.  caller might try catch the exception and it can still receive data close to correct
			yield return src;
			if (!isContinuous && !isEmpty) {
				throw new ArgumentException($"Cannot add this date level item since it will break the continuity of dates in the series.  Adjust its start date and end date to fix");
			}
		}

		/// <summary>
		/// Updates interval values within a specified range, automatically splitting intervals as needed.
		/// Only existing intervals overlapping the range are modified; no new intervals are created if the range has no overlap.
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="series">The interval series to update.</param>
		/// <param name="modify">Action to apply to modify interval values within the range.</param>
		/// <param name="clone">Function to create a copy of an interval when splitting is required.</param>
		/// <param name="start">The start of the update range (inclusive).</param>
		/// <param name="end">The end of the update range (inclusive).</param>
		/// <exception cref="ArgumentException">Thrown when start &gt; end.</exception>
		public static void Update<T, K>(this ICollection<T> series, Action<T> modify, Func<T, T> clone, K start, K end)
			where T : IClosedInterval<K> where K : IComparable<K> {
			if (start.CompareTo(end) > 0) {
				throw new ArgumentException("Start date cannot be greater than end date");
			}
			foreach (var current in series.ToArray()) {
				if (end.CompareTo(current.StartInclusive) < 0 || current.EndInclusive.CompareTo(start) < 0) {
					// no overlap
					continue;
				} else if (start.CompareTo(current.StartInclusive) <= 0 && current.EndInclusive.CompareTo(end) <= 0) {
					// new value overlap current
					modify(current);
				} else if (current.StartInclusive.CompareTo(start) < 0 && end.CompareTo(current.EndInclusive) < 0) {
					// current overlap new value
					var after = clone(current);
					after.StartInclusive = T.Next(end);
					after.EndInclusive = current.EndInclusive;
					series.Add(after);

					var newItem = clone(current);
					modify(newItem);
					newItem.StartInclusive = start;
					newItem.EndInclusive = end;
					series.Add(newItem);

					current.EndInclusive = T.Previous(start);
				} else if (start.CompareTo(current.StartInclusive) <= 0 && current.StartInclusive.CompareTo(end) <= 0 && end.CompareTo(current.EndInclusive) < 0) {
					var newItem = clone(current);
					modify(newItem);
					newItem.StartInclusive = current.StartInclusive;
					newItem.EndInclusive = end;
					series.Add(newItem);
					current.StartInclusive = T.Next(end);
				} else if (current.StartInclusive.CompareTo(start) < 0 && start.CompareTo(current.EndInclusive) <= 0 && end.CompareTo(current.EndInclusive) >= 0) {
					var newItem = clone(current);
					modify(newItem);
					newItem.StartInclusive = start;
					newItem.EndInclusive = current.EndInclusive;
					series.Add(newItem);
					current.EndInclusive = T.Previous(start);
				}
			}
		}

		/// <summary>
		/// Trims intervals to start at a new start boundary, removing any data before the new start.
		/// Intervals entirely before the new start are removed; intervals spanning the boundary are adjusted.
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="series">The interval series to trim.</param>
		/// <param name="newStart">The new start boundary.</param>
		/// <returns>The trimmed interval series.</returns>
		public static IEnumerable<T> TrimStart<T, K>(this IEnumerable<T> series, K newStart)
			where T : IClosedInterval<K>
			where K : IComparable<K> {
			foreach (var item in series) {
				if (item.EndInclusive.CompareTo(newStart) < 0) {
					continue;
				} else if (item.StartInclusive.CompareTo(newStart) < 0) {
					item.StartInclusive = newStart;
				}
				yield return item;
			}
		}

		/// <summary>
		/// Trims intervals to end at a new end boundary, removing any data after the new end.
		/// Intervals entirely after the new end are removed; intervals spanning the boundary are adjusted.
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="series">The interval series to trim.</param>
		/// <param name="newEnd">The new end boundary.</param>
		/// <returns>The trimmed interval series.</returns>
		public static IEnumerable<T> TrimEnd<T, K>(this IEnumerable<T> series, K newEnd)
			where T : IClosedInterval<K>
			where K : IComparable<K> {
			foreach (var item in series) {
				if (item.StartInclusive.CompareTo(newEnd) > 0) {
					continue;
				} else if (item.EndInclusive.CompareTo(newEnd) > 0) {
					item.EndInclusive = newEnd;
				}
				yield return item;
			}
		}

		/// <summary>
		/// Consolidates adjacent intervals with matching values into single intervals.
		/// The series is sorted by start before processing. Adjacent intervals where <paramref name="isEqual"/> returns true are merged.
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="source">The interval series to rebuild.</param>
		/// <param name="isEqual">Function to determine if two intervals have equal values and should be merged.</param>
		/// <returns>The consolidated interval series.</returns>
		public static IEnumerable<T> Rebuild<T, K>(this IEnumerable<T> source, Func<T, T, bool> isEqual)
			where T : IClosedInterval<K>
			where K : IComparable<K> {
			var items = source.OrderBy(x => x.StartInclusive).ToArray();
			T? current = default;
			foreach (var item in items) {
				if (current == null) {
					current = item;
				} else {
					if (isEqual(current, item)) {
						current.EndInclusive = item.EndInclusive;
					} else {
						current.EndInclusive = T.Previous(item.StartInclusive);
						var result = current;
						yield return result;
						current = item;
					}
				}
			}
			if (current != null) {
				yield return current;
			}
		}

		/// <summary>
		/// Merges overlapping or adjacent intervals in the series, assuming all intervals represent the same logical value.
		/// The series is sorted by start before processing.
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="source">The interval series to merge.</param>
		/// <returns>The merged interval series with no overlaps.</returns>
		public static IEnumerable<T> Merge<T, K>(this IEnumerable<T> source)
			where T : IClosedInterval<K>
			where K : IComparable<K> {

			T? current = default;
			foreach (var item in source.OrderBy(x => x.StartInclusive)) {
				if (current == null) {
					current = item;
				} else if (item.EndInclusive.CompareTo(current.EndInclusive) <= 0) {
					// item is completely within current,
					continue;
				} else if (item.StartInclusive.CompareTo(T.Next(current.EndInclusive)) <= 0) {
					// the item overlaps current
					current.EndInclusive = item.EndInclusive;
					continue;
				} else {
					yield return current;
					current = item;
				}
			}
			if (current != null) {
				yield return current;
			}
		}

		/// <summary>
		/// Finds the interval containing the specified key, throwing an exception if not found.
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="items">The interval series to search.</param>
		/// <param name="key">The key to find.</param>
		/// <returns>The interval containing the key.</returns>
		/// <exception cref="ArgumentException">Thrown when no interval contains the key.</exception>
		public static T FindRequired<T, K>(this IEnumerable<T> items, K key) where T : IClosedInterval<K> where K : IComparable<K>
			=> Find<T, K>(items, key) ?? throw new ArgumentException($"Cannot find an interval that overlaps {key}");

		/// <summary>
		/// Finds the interval containing the specified key, or null if not found.
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="items">The interval series to search.</param>
		/// <param name="key">The key to find.</param>
		/// <returns>The interval containing the key, or null if not found.</returns>
		public static T? Find<T, K>(this IEnumerable<T> items, K key) where T : IClosedInterval<K> where K : IComparable<K> {
			foreach (var item in items) {
				if (key.IsBetweenInclusive(item.StartInclusive, item.EndInclusive)) {
					return item;
				}
			}
			return default;
		}

		/// <summary>
		/// Finds all intervals that overlap with the specified range [start, end].
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="source">The interval series to search.</param>
		/// <param name="start">The start of the search range (inclusive).</param>
		/// <param name="end">The end of the search range (inclusive).</param>
		/// <returns>All intervals that overlap with the specified range.</returns>
		public static IEnumerable<T> Find<T, K>(this IEnumerable<T> source, K start, K end)
			where T : IClosedInterval<K> where K : IComparable<K> {
			return source.Where(args => !(start.IsGreaterThan(args.EndInclusive) || end.IsLessThan(args.StartInclusive)));
		}

		/// <summary>
		/// Verifies that the interval series is continuous (no gaps) and non-overlapping.
		/// </summary>
		/// <typeparam name="T">The interval type.</typeparam>
		/// <typeparam name="K">The type of interval boundaries.</typeparam>
		/// <param name="series">The interval series to verify.</param>
		/// <param name="throwException">If true, throws <see cref="IntervalException"/> on failure; otherwise, returns false.</param>
		/// <returns>True if the series is valid; false if invalid and <paramref name="throwException"/> is false.</returns>
		/// <exception cref="IntervalException">Thrown when the series is invalid and <paramref name="throwException"/> is true.</exception>
		public static bool Verify<T, K>(this IEnumerable<T> series, bool throwException)
			where T : IClosedInterval<K>
			where K : IComparable<K> {
			T? previous = default;
			foreach (var item in series.OrderBy(x => x.StartInclusive)) {
				if (item.StartInclusive.IsGreaterThan(item.EndInclusive)) {
					if (throwException) {
						throw new IntervalException($"Start date is greater than end date");
					} else {
						return false;
					}
				} else if (previous != null) {
					if (previous.EndInclusive.IsGreaterThanOrEqualTo(item.StartInclusive)) {
						if (throwException) {
							throw new IntervalException($"Start date overlaps with previous end date");
						} else {
							return false;
						}
					} else if (T.Next(previous.EndInclusive).IsLessThan(item.StartInclusive)) {
						if (throwException) {
							throw new IntervalException($"Start date is not continuous from previous end date");
						} else {
							return false;
						}
					}
					return false;
				}
				previous = item;
			}
			return true;
		}

		/// <summary>
		/// Joins two interval series, creating new intervals where they overlap.
		/// Both series should be continuous. The result contains intervals for each overlapping region.
		/// </summary>
		/// <typeparam name="TLeft">The type of intervals in the left series.</typeparam>
		/// <typeparam name="TRight">The type of intervals in the right series.</typeparam>
		/// <typeparam name="TResult">The type of intervals in the result.</typeparam>
		/// <typeparam name="TKey">The type of interval boundaries.</typeparam>
		/// <param name="leftSeries">The left interval series.</param>
		/// <param name="rightSeries">The right interval series.</param>
		/// <param name="convert">Function to create a result interval from overlapping left and right intervals.</param>
		/// <returns>A series of intervals representing the joined regions.</returns>
		public static IEnumerable<TResult> Join<TLeft, TRight, TResult, TKey>(this IEnumerable<TLeft> leftSeries, IEnumerable<TRight> rightSeries, Func<TLeft, TRight, TResult> convert)
			where TKey : IComparable<TKey>
			where TLeft : IClosedInterval<TKey>
			where TRight : IClosedInterval<TKey>
			where TResult : IClosedInterval<TKey> {
			var leftStack = new Stack<TLeft>(leftSeries.OrderByDescending(x => x.StartInclusive));
			var rightStack = new Stack<TRight>(rightSeries.OrderByDescending(x => x.StartInclusive));
			while (leftStack.Count > 0 && rightStack.Count > 0) {
				var left = leftStack.Pop();
				var right = rightStack.Pop();
				if (TryCombine(left, right, out TResult? result)) {
					yield return result;
				}
				var compare = left.EndInclusive.CompareTo(right.EndInclusive);
				if (compare > 0) {
					leftStack.Push(left);
				} else if (compare < 0) {
					rightStack.Push(right);
				}
			}

			bool TryCombine(TLeft left, TRight right, [NotNullWhen(true)] out TResult? result) {
				if (left.EndInclusive.IsLessThan(right.StartInclusive) || right.EndInclusive.IsLessThan(left.StartInclusive)) {
					result = default;
					return false;
				}
				result = convert(left, right);
				result.StartInclusive = left.StartInclusive.IsGreaterThanOrEqualTo(right.StartInclusive) ? left.StartInclusive : right.StartInclusive;
				result.EndInclusive = left.EndInclusive.IsLessThanOrEqualTo(right.EndInclusive) ? left.EndInclusive : right.EndInclusive;
				return true;
			}
		}
	}
}
