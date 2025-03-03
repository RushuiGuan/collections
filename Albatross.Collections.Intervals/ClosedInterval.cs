namespace Albatross.Collections.Intervals {
	public interface IClosedInterval<T> where T : IComparable<T> {
		T StartInclusive { get; set; }
		T EndInclusive { get; set; }
		T Next(T value);
		T Previous(T value);
	}

	public abstract class ClosedInterval<T, V> : IClosedInterval<T> where T : IComparable<T> where V : ICloneable, IEquatable<V> {
		public ClosedInterval(T startInclusive, T endInclusive) {
			StartInclusive = startInclusive;
			EndInclusive = endInclusive;
		}

		public T StartInclusive { get; set; }
		public T EndInclusive { get; set; }
		public abstract T Next(T value);
		public abstract T Previous(T value);
		public V? Value { get; set; }
	}
}
