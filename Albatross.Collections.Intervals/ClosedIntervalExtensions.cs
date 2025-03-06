namespace Albatross.Collections.Intervals {
	public static class ClosedIntervalExtensions {
		public static bool IsValid<T>(this IClosedInterval<T> interval) where T : IComparable<T> {
			return interval.StartInclusive.CompareTo(interval.EndInclusive) <= 0;
		}
		/// <summary>
		/// Use the method to create a continuous series of non overlapping interval data.  The input series doesn't need to be sorted and the resulting
		/// series will not be sorted either.  This method is suitable for series with small number of items.  The time complexity is always O(n).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="series"></param>
		/// <param name="src"></param>
		/// <param name="isEqual"></param>
		/// <param name="clone"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
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
				} else if (src.EndInclusive.CompareTo(T.Previous(item.StartInclusive)) == 0) {
					isContinuous = true;
					if (isEqual(src, item)) {
						item.StartInclusive = src.StartInclusive;
						src = item;
					} else {
						yield return item;
					}
				} else if (src.StartInclusive.CompareTo(T.Next(item.EndInclusive)) == 0) {
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
		/// This function will attempt to update date level values between <paramref name="start"/> and <paramref name="endDate"/>.  If the endDate is not specified, 
		/// the function will find the next record in the series and set the end date to the day before its start date.  If the next record does not exist, the end date 
		/// will be set to the max end date. The function will update only.  It will not insert records if no existing records that overlap the specified date range 
		/// are found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <param name="series"></param>
		/// <param name="clone">function pointer to clone an instance of T</param>
		/// <param name="modify">action pointer to modify the value</param>
		/// <param name="start"></param>
		/// <param name="endDate"></param>
		/// <exception cref="ArgumentException"></exception>
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
		/// Provided a date level series data for a single entity, the method will set the start date of the series to the new start date by trimming
		/// the any data prior to the new start date.  The method assume that the series is correctly built.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="series"></param>
		/// <param name="newStart"></param>
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
					}
				}
			}
			if (current != null) {
				yield return current;
			}
		}

		public static T? Find<T, K>(this IEnumerable<T> items, K key) where T : IClosedInterval<K> where K : IComparable<K> {
			foreach (var item in items) {
				if (key.IsBetweenInclusive(item.StartInclusive, item.EndInclusive)) {
					return item;
				}
			}
			return default;
		}

		public static IEnumerable<T> Find<T, K>(this IEnumerable<T> source, K start, K end)
			where T : IClosedInterval<K> where K : IComparable<K> {
			return source.Where(args => !(start.IsGreaterThan(args.EndInclusive) || end.IsLessThan(args.StartInclusive)));
			//return source.Where(args => !(start.CompareTo(args.EndInclusive) > 0 || end.CompareTo(args.StartInclusive) < 0));
		}
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
					} else if (T.Previous(previous.EndInclusive).IsLessThan(item.StartInclusive)) {
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
	}
}