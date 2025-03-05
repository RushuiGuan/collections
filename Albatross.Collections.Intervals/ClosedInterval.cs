namespace Albatross.Collections.Intervals {
	public interface IClosedInterval<K> where K : IComparable<K> {
		K StartInclusive { get; set; }
		K EndInclusive { get; set; }
		K Next(K value);
		K Previous(K value);
	}

	public abstract class ClosedInterval<K, V> : IClosedInterval<K> where K : IComparable<K> where V : ICloneable, IEquatable<V> {
		public ClosedInterval(K startInclusive, K endInclusive) {
			StartInclusive = startInclusive;
			EndInclusive = endInclusive;
		}

		public K StartInclusive { get; set; }
		public K EndInclusive { get; set; }
		public abstract K Next(K value);
		public abstract K Previous(K value);
		public V? Value { get; set; }
	}
}
