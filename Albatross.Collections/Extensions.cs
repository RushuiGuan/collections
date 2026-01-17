using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Albatross.Collections {
	/// <summary>
	/// Provides extension methods for common collection operations including batching, dictionary utilities, and list manipulation.
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// Splits an enumerable into batches of a specified size. The last batch may contain fewer items if the total count is not evenly divisible.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="items">The source enumerable to batch.</param>
		/// <param name="size">The maximum number of items per batch. Must be greater than 0.</param>
		/// <returns>An enumerable of arrays, each containing up to <paramref name="size"/> elements.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="size"/> is 0.</exception>
		public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> items, int size) {
			if (size == 0) { throw new ArgumentException("Batch size cannot be 0"); }
			var array = new T[size];
			int index = 0;
			foreach (var item in items) {
				array[index] = item;
				index++;
				if (index == size) {
					yield return array;
					array = new T[size];
					index = 0;
				}
			}
			if (index != 0) {
				var copy = new T[index];
				Array.Copy(array, copy, index);
				yield return copy;
			}
		}

		/// <summary>
		/// Adds multiple items to a collection. Useful for collection types that don't have a built-in AddRange method.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="collection">The target collection.</param>
		/// <param name="items">The items to add.</param>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items) {
			foreach (var item in items) {
				collection.Add(item);
			}
		}

		/// <summary>
		/// Executes an action on each item in the enumerable. Does nothing if the enumerable is null.
		/// </summary>
		/// <typeparam name="T">The type of elements.</typeparam>
		/// <param name="items">The source enumerable.</param>
		/// <param name="action">The action to execute on each item.</param>
		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action) {
			if (items != null) {
				foreach (var item in items) {
					action(item);
				}
			}
		}

		/// <summary>
		/// Gets a value from the dictionary by key, or adds it using the factory function if the key doesn't exist.
		/// </summary>
		/// <typeparam name="K">The type of keys in the dictionary.</typeparam>
		/// <typeparam name="T">The type of values in the dictionary.</typeparam>
		/// <param name="dict">The dictionary.</param>
		/// <param name="key">The key to look up.</param>
		/// <param name="func">A factory function to create the value if the key doesn't exist.</param>
		/// <returns>The existing value or the newly created value.</returns>
		public static T GetOrAdd<K, T>(this IDictionary<K, T> dict, K key, Func<T> func) {
			if (!dict.TryGetValue(key, out T value)) {
				value = func();
				dict.Add(key, value);
			}
			return value;
		}

		/// <summary>
		/// Attempts to get a value from the dictionary and remove it in a single operation.
		/// </summary>
		/// <typeparam name="K">The type of keys in the dictionary.</typeparam>
		/// <typeparam name="T">The type of values in the dictionary.</typeparam>
		/// <param name="dict">The dictionary.</param>
		/// <param name="key">The key to look up and remove.</param>
		/// <param name="value">When this method returns, contains the value associated with the key if found; otherwise, the default value.</param>
		/// <returns>True if the key was found and removed; otherwise, false.</returns>
		public static bool TryGetAndRemove<K, T>(this IDictionary<K, T> dict, K key, [NotNullWhen(true)] out T? value) where T : notnull {
			if (dict.TryGetValue(key, out value)) {
				dict.Remove(key);
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Gets an item from the collection matching the predicate, or adds a new item using the factory function if no match is found.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="list">The collection to search.</param>
		/// <param name="predicate">The predicate to match items against.</param>
		/// <param name="func">A factory function to create a new item if no match is found.</param>
		/// <returns>The matching item or the newly created item.</returns>
		public static T GetOrAdd<T>(this ICollection<T> list, Func<T, bool> predicate, Func<T> func) {
			var item = list.FirstOrDefault(predicate);
			if (item == null) {
				item = func();
				list.Add(item);
			}
			return item;
		}

		/// <summary>
		/// Attempts to find and remove the first item matching the predicate from the list.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="list">The list to search.</param>
		/// <param name="predicate">The predicate to match items against.</param>
		/// <param name="item">When this method returns, contains the matching item if found; otherwise, the default value.</param>
		/// <returns>True if a matching item was found and removed; otherwise, false.</returns>
		public static bool TryGetOneAndRemove<T>(this IList<T> list, Func<T, bool> predicate, [NotNullWhen(true)] out T? item) where T : notnull {
			for (var i = 0; i < list.Count; i++) {
				if (predicate(list[i])) {
					item = list[i];
					list.RemoveAt(i);
					return true;
				}
			}
			item = default;
			return false;
		}

		/// <summary>
		/// The default list size threshold for switching between removal algorithms in <see cref="RemoveAny{T}"/>.
		/// </summary>
		public const int ListItemRemovalAlgoCutoff = 100;

		/// <summary>
		/// Removes all items matching the predicate from the list, automatically selecting the optimal algorithm based on list size.
		/// Uses <see cref="RemoveAny_FromRear{T}"/> for lists with count &lt;= <paramref name="algoCutoff"/>, otherwise uses <see cref="RemoveAny_WithNewList{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="list">The list to remove items from.</param>
		/// <param name="predicate">The predicate to match items for removal.</param>
		/// <param name="algoCutoff">The list size threshold for algorithm selection. Defaults to <see cref="ListItemRemovalAlgoCutoff"/>.</param>
		/// <returns>A list containing the removed items.</returns>
		public static IList<T> RemoveAny<T>(this IList<T> list, Predicate<T> predicate, int algoCutoff = ListItemRemovalAlgoCutoff) {
			if (list.Count > algoCutoff) {
				return list.RemoveAny_WithNewList(predicate);
			} else {
				return list.RemoveAny_FromRear(predicate);
			}
		}

		/// <summary>
		/// Removes all items matching the predicate by iterating from the end of the list. O(n) complexity, efficient for smaller lists.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="list">The list to remove items from.</param>
		/// <param name="predicate">The predicate to match items for removal.</param>
		/// <returns>A list containing the removed items.</returns>
		public static IList<T> RemoveAny_FromRear<T>(this IList<T> list, Predicate<T> predicate) {
			var removed = new List<T>();
			for (int i = list.Count - 1; i >= 0; i--) {
				if (predicate(list[i])) {
					removed.Add(list[i]);
					list.RemoveAt(i);
				}
			}
			return removed;
		}

		/// <summary>
		/// Removes all items matching the predicate by creating a new list with non-matching items. O(n) complexity, better for large lists with many removals.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="list">The list to remove items from.</param>
		/// <param name="predicate">The predicate to match items for removal.</param>
		/// <returns>A list containing the removed items.</returns>
		public static IList<T> RemoveAny_WithNewList<T>(this IList<T> list, Predicate<T> predicate) {
			var removed = new List<T>();
			var newList = new List<T>();
			foreach (var item in list) {
				if (!predicate(item)) {
					newList.Add(item);
				} else {
					removed.Add(item);
				}
			}
			list.Clear();
			list.AddRange(newList);
			return removed;
		}

		/// <summary>
		/// Adds items to the collection only if they are not null.
		/// </summary>
		/// <typeparam name="T">The type of elements in the collection.</typeparam>
		/// <param name="collection">The target collection.</param>
		/// <param name="items">The items to add if not null.</param>
		/// <returns>The collection for method chaining.</returns>
		public static ICollection<T> AddIfNotNull<T>(this ICollection<T> collection, params T?[] items) {
			foreach (var item in items) {
				if (item != null) {
					collection.Add(item);
				}
			}
			return collection;
		}

		/// <summary>
		/// Concatenates enumerables, filtering out null values. For reference types.
		/// </summary>
		/// <typeparam name="T">The type of elements.</typeparam>
		/// <param name="first">The first enumerable.</param>
		/// <param name="second">Additional items to concatenate, nulls are filtered out.</param>
		/// <returns>A concatenated enumerable with null values removed.</returns>
		public static IEnumerable<T> UnionIfNotNull<T>(this IEnumerable<T> first, params IEnumerable<T?> second) where T : class {
			return first.Concat(second.Where(x => x is not null))!;
		}

		/// <summary>
		/// Concatenates enumerables, filtering out null values. For nullable value types.
		/// </summary>
		/// <typeparam name="T">The underlying value type.</typeparam>
		/// <param name="first">The first enumerable.</param>
		/// <param name="second">Additional nullable items to concatenate, nulls are filtered out.</param>
		/// <returns>A concatenated enumerable with null values removed.</returns>
		public static IEnumerable<T> UnionIfNotNull<T>(this IEnumerable<T> first, params IEnumerable<Nullable<T>> second) where T : struct {
			return first.Concat(second.Where(x => x is not null).Select(x => x!.Value));
		}

		/// <summary>
		/// Wraps a single item as an enumerable containing one element.
		/// </summary>
		/// <typeparam name="T">The type of the item.</typeparam>
		/// <param name="item">The item to wrap.</param>
		/// <returns>An enumerable containing the single item.</returns>
		public static IEnumerable<T> AsEnumerable<T>(this T item) {
			yield return item;
		}

		/// <summary>
		/// Gets a value from the dictionary if it exists and optionally matches a predicate. For reference types.
		/// </summary>
		/// <typeparam name="K">The type of keys in the dictionary.</typeparam>
		/// <typeparam name="T">The type of values in the dictionary.</typeparam>
		/// <param name="dict">The dictionary.</param>
		/// <param name="key">The key to look up.</param>
		/// <param name="predicate">Optional predicate the value must satisfy. If null, any value is returned.</param>
		/// <returns>The value if found and matching the predicate; otherwise, null.</returns>
		public static T? Where<K, T>(this IDictionary<K, T> dict, K key, Func<T, bool>? predicate = null) where T : class {
			if (dict.TryGetValue(key, out var value)) {
				if (predicate == null || predicate(value)) {
					return value;
				} else {
					return null;
				}
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a value from the dictionary if it exists and optionally matches a predicate. For value types.
		/// </summary>
		/// <typeparam name="K">The type of keys in the dictionary.</typeparam>
		/// <typeparam name="T">The type of values in the dictionary.</typeparam>
		/// <param name="dict">The dictionary.</param>
		/// <param name="key">The key to look up.</param>
		/// <param name="predicate">Optional predicate the value must satisfy. If null, any value is returned.</param>
		/// <returns>The value if found and matching the predicate; otherwise, null.</returns>
		public static T? WhereValue<K, T>(this IDictionary<K, T> dict, K key, Func<T, bool>? predicate = null) where T : struct {
			if (dict.TryGetValue(key, out var value)) {
				if (predicate == null || predicate(value)) {
					return value;
				} else {
					return null;
				}
			} else {
				return null;
			}
		}
	}
}
