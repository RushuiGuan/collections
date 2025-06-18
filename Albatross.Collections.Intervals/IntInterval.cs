namespace Albatross.Collections.Intervals {
	public record class IntInterval : IClosedInterval<int> {
		public int StartInclusive { get; set; }
		public int EndInclusive { get; set; }
		public static int Next(int value) {
			if (value == int.MaxValue) { throw new ArgumentOutOfRangeException("Max value reached"); }
			return value + 1;
		}
		public static int Previous(int value) {
			if (value == int.MinValue) { throw new ArgumentOutOfRangeException("Min value reached"); }
			return value - 1;
		}

	}

	public record class IntInterval<V> : IntInterval, IClosedInterval<int, V> where V : IEquatable<V> {
		public IntInterval(int start, int end, V value) {
			Value = value;
			this.StartInclusive = start;
			this.EndInclusive = end;
		}
		public V Value { get; set; }
	}

	public static class IntIntervalExtensions {
		public static IEnumerable<IntInterval<V>> Insert<V>(this IEnumerable<IntInterval<V>> series, IntInterval<V> src, Func<V, V> clone) where V : IEquatable<V>
			=> series.Insert<IntInterval<V>, int>(src,
				(a, b) => EqualityComparer<V>.Default.Equals(a.Value, b.Value),
				a => new IntInterval<V>(a.StartInclusive, a.EndInclusive, clone(a.Value)));

		public static void Update<V>(this ICollection<IntInterval<V>> series, Action<IntInterval<V>> modify, int start, int end, Func<V, V> clone) where V : IEquatable<V>
			=> series.Update<IntInterval<V>, int>(modify, a => new IntInterval<V>(a.StartInclusive, a.EndInclusive, clone(a.Value)), start, end);
	}
}
