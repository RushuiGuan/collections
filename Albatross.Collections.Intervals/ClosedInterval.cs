namespace Albatross.Collections.Intervals {
	/// <summary>
	/// Represents a closed interval [start, end] where both boundaries are inclusive.
	/// Implementations must provide <see cref="Next"/> and <see cref="Previous"/> methods to define interval arithmetic.
	/// </summary>
	/// <typeparam name="K">The type of the interval boundaries. Must implement <see cref="IComparable{T}"/>.</typeparam>
	public interface IClosedInterval<K> where K : IComparable<K> {
		K StartInclusive { get; set; }
		K EndInclusive { get; set; }
		/// <summary>
		/// Returns the next value after the given value. Used for interval splitting and adjacency detection.
		/// </summary>
		/// <param name="value">The current value.</param>
		/// <returns>The next sequential value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is at the maximum boundary.</exception>
		static abstract K Next(K value);
		/// <summary>
		/// Returns the previous value before the given value. Used for interval splitting and adjacency detection.
		/// </summary>
		/// <param name="value">The current value.</param>
		/// <returns>The previous sequential value.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is at the minimum boundary.</exception>
		static abstract K Previous(K value);
	}

	/// <summary>
	/// Represents a closed interval with an associated value. Used for interval series where each range maps to a value.
	/// </summary>
	/// <typeparam name="K">The type of the interval boundaries.</typeparam>
	/// <typeparam name="V">The type of the value associated with the interval. Must implement <see cref="IEquatable{T}"/> for merge operations.</typeparam>
	public interface IClosedInterval<K, V> : IClosedInterval<K>
		where K : IComparable<K>
		where V : IEquatable<V> {
		V Value { get; set; }
	}
}
