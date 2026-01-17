using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Albatross.Collections {
	/// <summary>
	/// Provides extension methods for merging collections using full outer join semantics.
	/// </summary>
	public static class MergeExtensions {
		/// <summary>
		/// Performs a full outer join between two collections, invoking callbacks for matched items, items only in source, and items only in destination.
		/// Items with a default key value (e.g., 0 for int, null for reference types) are treated as new items and handled by <paramref name="notMatchedByDst"/>.
		/// </summary>
		/// <typeparam name="Src">The type of elements in the source collection.</typeparam>
		/// <typeparam name="Dst">The type of elements in the destination collection.</typeparam>
		/// <typeparam name="TKey">The type of the key used for matching. Must be non-null.</typeparam>
		/// <param name="dst">The destination collection (existing items).</param>
		/// <param name="src">The source collection (incoming items).</param>
		/// <param name="srcKeySelector">Function to extract the key from a source item.</param>
		/// <param name="dstKeySelector">Function to extract the key from a destination item.</param>
		/// <param name="matched">Callback invoked when a source item matches a destination item by key. Typically used for updates.</param>
		/// <param name="notMatchedByDst">Callback invoked for source items with no matching destination item. Typically used for inserts.</param>
		/// <param name="notMatchedBySrc">Callback invoked for destination items with no matching source item. Typically used for deletes.</param>
		public static void Merge<Src, Dst, TKey>(this IEnumerable<Dst> dst, IEnumerable<Src> src,
			Func<Src, TKey> srcKeySelector, Func<Dst, TKey> dstKeySelector,
			Action<Src, Dst>? matched, Action<Src>? notMatchedByDst, Action<Dst>? notMatchedBySrc) where TKey : notnull {
			var dstArray = dst.ToArray();
			if (src == null) { src = new Src[0]; }
			var srcDict = new Dictionary<TKey, Src>();
			var newItems = new List<Src>();

			foreach (var item in src) {
				TKey key = srcKeySelector(item);
				if (object.Equals(key, default(TKey))) {
					newItems.Add(item);
				} else {
					srcDict.Add(key, item);
				}
			}
			foreach (var item in dstArray) {
				TKey key = dstKeySelector(item);
				if (srcDict.TryGetValue(key, out Src srcItem)) {
					matched?.Invoke(srcItem, item);
					srcDict.Remove(key);
				} else {
					notMatchedBySrc?.Invoke(item);
				}
			}
			foreach (var item in srcDict.Values) {
				notMatchedByDst?.Invoke(item);
			}
			foreach (var item in newItems) {
				notMatchedByDst?.Invoke(item);
			}
		}

		/// <summary>
		/// Asynchronously performs a full outer join between two collections, invoking async callbacks for matched items, items only in source, and items only in destination.
		/// Items with a default key value are treated as new items and handled by <paramref name="notMatchedByDst"/>.
		/// </summary>
		/// <typeparam name="Src">The type of elements in the source collection.</typeparam>
		/// <typeparam name="Dst">The type of elements in the destination collection.</typeparam>
		/// <typeparam name="TKey">The type of the key used for matching. Must be non-null.</typeparam>
		/// <param name="dst">The destination collection (existing items).</param>
		/// <param name="src">The source collection (incoming items).</param>
		/// <param name="srcKeySelector">Function to extract the key from a source item.</param>
		/// <param name="dstKeySelector">Function to extract the key from a destination item.</param>
		/// <param name="matched">Async callback invoked when a source item matches a destination item by key.</param>
		/// <param name="notMatchedByDst">Async callback invoked for source items with no matching destination item.</param>
		/// <param name="notMatchedBySrc">Async callback invoked for destination items with no matching source item.</param>
		public static async Task MergeAsync<Src, Dst, TKey>(this IEnumerable<Dst> dst, IEnumerable<Src> src,
			Func<Src, TKey> srcKeySelector, Func<Dst, TKey> dstKeySelector,
			Func<Src, Dst, Task>? matched,
			Func<Src, Task>? notMatchedByDst,
			Func<Dst, Task>? notMatchedBySrc) where TKey : notnull {

			var dstArray = dst.ToArray();
			if (src == null) { src = new Src[0]; }
			var srcDict = new Dictionary<TKey, Src>();
			var newItems = new List<Src>();

			foreach (var item in src) {
				TKey key = srcKeySelector(item);
				if (object.Equals(key, default(TKey))) {
					newItems.Add(item);
				} else {
					srcDict.Add(key, item);
				}
			}
			foreach (var item in dstArray) {
				TKey key = dstKeySelector(item);
				if (srcDict.TryGetValue(key, out Src? srcItem)) {
					if (matched != null) {
						await matched(srcItem, item);
					}
					srcDict.Remove(key);
				} else {
					if (notMatchedBySrc != null) {
						await notMatchedBySrc(item);
					}
				}
			}
			if (notMatchedByDst != null) {
				foreach (var item in srcDict.Values) {
					await notMatchedByDst(item);
				}
				foreach (var item in newItems) {
					await notMatchedByDst(item);
				}
			}
		}
	}
}
