using System;
using System.Collections.Generic;

namespace Albatross.Collections {
	/// <summary>
	/// Provides binary search extension methods for sorted collections.
	/// </summary>
	public static class SearchExtensions {
		/// <summary>
		/// Performs a binary search to find the first element with a key greater than or equal to the target. For value types (structs).
		/// </summary>
		/// <typeparam name="Record">The type of elements in the list. Must be a value type.</typeparam>
		/// <typeparam name="Key">The type of the key to compare.</typeparam>
		/// <param name="list">A sorted list to search. Must be sorted in ascending order by the key.</param>
		/// <param name="key">The target key to search for.</param>
		/// <param name="getKey">A function to extract the key from each element.</param>
		/// <returns>The first element with a key &gt;= <paramref name="key"/>, or null if no such element exists.</returns>
		public static Record? BinarySearchFirstValueGreaterOrEqual<Record, Key>(this IList<Record> list, Key key, Func<Record, Key> getKey)
			where Record : struct where Key : IComparable<Key> {
			int left = 0;
			int right = list.Count - 1;
			Record? result = default;

			if (key.CompareTo(getKey(list[0])) <= 0) {
				return list[0];
			} else if (key.CompareTo(getKey(list[list.Count - 1])) > 0) {
				return default;
			}
			while (left <= right) {
				int mid = left + (right - left) / 2;
				Record item = list[mid];
				Key value = getKey(item);

				if (value.CompareTo(key) >= 0) {
					result = item;
					right = mid - 1;
				} else {
					left = mid + 1;
				}
			}
			return result;
		}

		/// <summary>
		/// Performs a binary search to find the first element with a key less than or equal to the target. For value types (structs).
		/// </summary>
		/// <typeparam name="Record">The type of elements in the list. Must be a value type.</typeparam>
		/// <typeparam name="Key">The type of the key to compare.</typeparam>
		/// <param name="list">A sorted list to search. Must be sorted in ascending order by the key.</param>
		/// <param name="key">The target key to search for.</param>
		/// <param name="getKey">A function to extract the key from each element.</param>
		/// <returns>The first element with a key &lt;= <paramref name="key"/>, or null if no such element exists.</returns>
		public static Record? BinarySearchFirstValueLessOrEqual<Record, Key>(this IList<Record> list, Key key, Func<Record, Key> getKey)
			where Record : struct
			where Key : IComparable<Key> {
			int left = 0;
			int right = list.Count - 1;
			Record? result = default;

			if (key.CompareTo(getKey(list[0])) < 0) {
				return default;
			} else if (key.CompareTo(getKey(list[list.Count - 1])) >= 0) {
				return list[list.Count - 1];
			}
			while (left <= right) {
				int mid = left + (right - left) / 2;
				Record item = list[mid];
				Key value = getKey(item);

				if (value.CompareTo(key) <= 0) {
					result = item;
					left = mid + 1;
				} else {
					right = mid - 1;
				}
			}
			return result;
		}

		/// <summary>
		/// Performs a binary search to find the first element with a key greater than or equal to the target. For reference types (classes).
		/// </summary>
		/// <typeparam name="Record">The type of elements in the list. Must be a reference type.</typeparam>
		/// <typeparam name="Key">The type of the key to compare.</typeparam>
		/// <param name="list">A sorted list to search. Must be sorted in ascending order by the key.</param>
		/// <param name="key">The target key to search for.</param>
		/// <param name="getKey">A function to extract the key from each element.</param>
		/// <returns>The first element with a key &gt;= <paramref name="key"/>, or null if no such element exists.</returns>
		public static Record? BinarySearchFirstGreaterOrEqual<Record, Key>(this IList<Record> list, Key key, Func<Record, Key> getKey)
			where Record : class where Key : IComparable<Key> {
			int left = 0;
			int right = list.Count - 1;
			Record? result = default;

			if (key.CompareTo(getKey(list[0])) <= 0) {
				return list[0];
			} else if (key.CompareTo(getKey(list[list.Count - 1])) > 0) {
				return default;
			}
			while (left <= right) {
				int mid = left + (right - left) / 2;
				Record item = list[mid];
				Key value = getKey(item);

				if (value.CompareTo(key) >= 0) {
					result = item;
					right = mid - 1;
				} else {
					left = mid + 1;
				}
			}
			return result;
		}

		/// <summary>
		/// Performs a binary search to find the first element with a key less than or equal to the target. For reference types (classes).
		/// </summary>
		/// <typeparam name="Record">The type of elements in the list. Must be a reference type.</typeparam>
		/// <typeparam name="Key">The type of the key to compare.</typeparam>
		/// <param name="list">A sorted list to search. Must be sorted in ascending order by the key.</param>
		/// <param name="key">The target key to search for.</param>
		/// <param name="getKey">A function to extract the key from each element.</param>
		/// <returns>The first element with a key &lt;= <paramref name="key"/>, or null if no such element exists.</returns>
		public static Record? BinarySearchFirstLessOrEqual<Record, Key>(this IList<Record> list, Key key, Func<Record, Key> getKey)
			where Record : class
			where Key : IComparable<Key> {
			int left = 0;
			int right = list.Count - 1;
			Record? result = default;

			if (key.CompareTo(getKey(list[0])) < 0) {
				return default;
			} else if (key.CompareTo(getKey(list[list.Count - 1])) >= 0) {
				return list[list.Count - 1];
			}
			while (left <= right) {
				int mid = left + (right - left) / 2;
				Record item = list[mid];
				Key value = getKey(item);

				if (value.CompareTo(key) <= 0) {
					result = item;
					left = mid + 1;
				} else {
					right = mid - 1;
				}
			}
			return result;
		}
	}
}
