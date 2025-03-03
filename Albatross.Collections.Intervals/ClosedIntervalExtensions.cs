namespace Albatross.Collections.Intervals {
	public static class ClosedIntervalExtensions {
		public static bool IsValid<T>(this IClosedInterval<T> interval) where T : IComparable<T> {
			return interval.StartInclusive.CompareTo(interval.EndInclusive) <= 0;
		}
		public static IEnumerable<T> Set<T, K>(this IEnumerable<T> series, T src, Func<T, T, bool> isEqual, Func<T, T> clone)
			where T : IClosedInterval<K> where K : IComparable<K> {
			if (!src.IsValid()) {
				throw new ArgumentException("StartInclusive cannot be greater than EndInclusive");
			}
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
						after.StartInclusive = src.Next(src.EndInclusive);
						// note that item.EndInclusive is modified.  therefore after.EndInclusive should be set first.
						after.EndInclusive = item.EndInclusive;
						item.EndInclusive = src.Previous(src.StartInclusive);
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
						item.StartInclusive = src.Next(src.EndInclusive);
						yield return item;
					}
				} else if (item.StartInclusive.CompareTo(src.StartInclusive) < 0 && src.StartInclusive.CompareTo(item.EndInclusive) <= 0 && item.EndInclusive.CompareTo(src.EndInclusive) <= 0) {
					isContinuous = true;
					if (isEqual(src, item)) {
						item.EndInclusive = src.EndInclusive;
						src = item;
					} else {
						item.EndInclusive = src.Previous(src.StartInclusive);
						yield return item;
					}
				} else if (src.EndInclusive.Equals(item.Previous(item.StartInclusive))) {
					isContinuous = true;
					if (isEqual(src, item)) {
						item.StartInclusive = src.StartInclusive;
						src = item;
					} else {
						yield return item;
					}
				} else if (src.StartInclusive.Equals(item.Next(item.EndInclusive))) {
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
		public static void UpdateDateLevel<T>(this ICollection<T> series, Action<T> modify, DateOnly start, DateOnly end, bool rebuild = true)
			where T : DateLevelEntity {
			if (start > end) {
				throw new ArgumentException("Start date cannot be greater than end date");
			}
			foreach (var current in series.ToArray()) {
				if (end < current.StartInclusive || current.EndInclusive < start) {
					// no overlap
					continue;
				} else if (start <= current.StartInclusive && current.EndInclusive <= end) {
					// new value overlap current
					modify(current);
				} else if (current.StartInclusive < start && end < current.EndInclusive) {
					// current overlap new value
					var after = (T)current.Clone();
					after.StartInclusive = end.AddDays(1);
					after.EndInclusive = current.EndInclusive;
					series.Add(after);

					var newItem = (T)current.Clone();
					modify(newItem);
					newItem.StartInclusive = start;
					newItem.EndInclusive = end;
					series.Add(newItem);

					current.EndInclusive = start.AddDays(-1);
				} else if (start <= current.StartInclusive && current.StartInclusive <= end && end < current.EndInclusive) {
					var newItem = (T)current.Clone();
					modify(newItem);
					newItem.StartInclusive = current.StartInclusive;
					newItem.EndInclusive = end;
					series.Add(newItem);
					current.StartInclusive = end.AddDays(1);
				} else if (current.StartInclusive < start && start <= current.EndInclusive && end >= current.EndInclusive) {
					var newItem = (T)current.Clone();
					modify(newItem);
					newItem.StartInclusive = start;
					newItem.EndInclusive = current.EndInclusive;
					series.Add(newItem);
					current.EndInclusive = start.AddDays(-1);
				}
			}
			if (rebuild) {
				var newSeries = series.RebuildDateLevelSeries();
				series.Clear();
				foreach (var item in newSeries) {
					series.Add(item);
				}
			}
		}

		/// <summary>
		/// Provided a date level series data for a single entity, the method will set the start date of the series to the new start date by trimming
		/// the any data prior to the new start date.  The method assume that the series is correctly built.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="series"></param>
		/// <param name="newStartDate"></param>
		public static IEnumerable<T> TrimStart<T>(this IEnumerable<T> series, DateOnly newStartDate) where T : DateLevelEntity {
			foreach (var item in series) {
				if (item.EndInclusive < newStartDate) {
					continue;
				} else if (item.StartInclusive < newStartDate) {
					item.StartInclusive = newStartDate;
				}
				yield return item;
			}
		}
		public static IEnumerable<T> TrimEnd<T>(this IEnumerable<T> series, DateOnly newEndDate) where T : DateLevelEntity {
			foreach (var item in series) {
				if (item.StartInclusive > newEndDate) {
					continue;
				} else if (item.EndInclusive > newEndDate) {
					item.EndInclusive = newEndDate;
				}
				yield return item;
			}
		}

		/// <summary>
		/// Provided a date level series data for a single entity, the method will rebuild the end dates and remove items if necessary
		/// The method doesnot care if the current data has incorrect end dates.  It will use the key and the start date to rebuild the
		/// end of the date level series.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="remove"></param>
		/// <returns></returns>
		public static IEnumerable<T> RebuildDateLevelSeries<T, K>(this IEnumerable<T> source)
			where T : DateLevelEntity<K>
			where K : IEquatable<K> {
			var groups = source.GroupBy(x => x.Key);
			foreach (var group in groups) {
				foreach (var item in RebuildDateLevelSeries<T>(group)) {
					yield return item;
				}
			}
		}

		public static IEnumerable<T> RebuildDateLevelSeries<T>(this IEnumerable<T> source)
			where T : DateLevelEntity {
			var items = source.OrderBy(x => x.StartInclusive).ToArray();
			T? current = null;
			foreach (var item in items) {
				if (current == null) {
					current = item;
				} else {
					if (current.HasSameValue(item)) {
						current.EndInclusive = item.EndInclusive;
					} else {
						current.EndInclusive = item.StartInclusive.AddDays(-1);
						yield return current;
					}
				}
			}
			if (current != null) {
				yield return current;
			}
		}

		public static T? Find<T, K>(this IEnumerable<T> items, K key) where T : IClosedInterval<K> where K : IComparable<K> {
			foreach (var item in items) {
				if (item.StartInclusive.CompareTo(key) <= 0 && item.EndInclusive.CompareTo(key) >= 0) {
					return item;
				}
			}
			return default;
		}

		public static IEnumerable<T> Find<T, K>(this IEnumerable<T> source, K start, K end)
			where T : IClosedInterval<K> where K : IComparable<K> {
			return source.Where(args => !(start > args.EndInclusive || end < args.StartInclusive));
		}

		public static bool VerifySeries<T>(this IEnumerable<T> series, bool throwException) where T : DateLevelEntity {
			T? previous = null;
			foreach (var item in series.OrderBy(x => x.StartInclusive)) {
				if (item.StartInclusive > item.EndInclusive) {
					if (throwException) {
						throw new DateLevelException(item, $"Start date is greater than end date");
					} else {
						return false;
					}
				} else if (previous != null) {
					if (previous.EndInclusive >= item.StartInclusive) {
						if (throwException) {
							throw new DateLevelException(item, $"Start date overlaps with previous end date");
						} else {
							return false;
						}
					} else if (previous.EndInclusive.AddDays(1) < item.StartInclusive) {
						if (throwException) {
							throw new DateLevelException(item, $"Start date is not continuous from previous end date");
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