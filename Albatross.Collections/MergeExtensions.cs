﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Albatross.Collections {
	public static class MergeExtensions {
		/// <summary>
		/// Perform a full outer join between 2 collections by comparing the key generated by the specified func delegates.  The method also accepts delegate parameters 
		/// that handles the join, left join and right join use cases.  The method doesn't consider the key value of default(TKey) a valid key value.  Therefore any 
		/// source item with the key value of default(TKey) will be handled by the notMatchedByDst delegate.
		/// </summary>
		/// <typeparam name="Src">The type of the source collection item</typeparam>
		/// <typeparam name="Dst">The type of the destination collection item</typeparam>
		/// <typeparam name="TKey">The type of the key that 2 collections join on</typeparam>
		/// <param name="dst">The destination collection</param>
		/// <param name="src">The source collection</param>
		/// <param name="srcKeySelector">The Func delegate that returns the TKey object from a Src item</param>
		/// <param name="dstKeySelector">The Func delegate that returns the TKey object from a Dst item</param>
		/// <param name="matched">The Action delegate that handle the use case when src and dst items have the matching keys</param>
		/// <param name="notMatchedByDst">The Action delegate that handle the use case when an item exists in the source collection but not in the destination collection</param>
		/// <param name="notMatchedBySrc">The Action delegate that handle the use case when an item exists in the destination collection but not in the source collection</param>
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