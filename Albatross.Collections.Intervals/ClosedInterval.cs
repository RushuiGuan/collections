﻿namespace Albatross.Collections.Intervals {
	public interface IClosedInterval<K> where K : IComparable<K> {
		K StartInclusive { get; set; }
		K EndInclusive { get; set; }
		static abstract K Next(K value);
		static abstract K Previous(K value);
	}

	public interface IClosedInterval<K, V> : IClosedInterval<K> 
		where K : IComparable<K> 
		where V : IEquatable<V> {
		V Value { get; set; }
	}
}
