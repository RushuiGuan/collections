namespace Albatross.Collections.Intervals {
	/// <summary>
	/// Represents a half-open interval [start, end) where the start is inclusive and the end is exclusive.
	/// </summary>
	/// <typeparam name="T">The type of the interval boundaries.</typeparam>
	public interface IOpenClosedInterval<T> where T : IComparable<T> {
		T StartInclusive { get; set; }
		T EndExclusive { get; set; }
	}

	/// <summary>
	/// A basic implementation of a half-open interval [start, end) with inclusive start and exclusive end.
	/// </summary>
	/// <typeparam name="T">The type of the interval boundaries.</typeparam>
	public class OpenClosedInterval<T> : IOpenClosedInterval<T> where T : IComparable<T> {
		public OpenClosedInterval(T startInclusive, T endExclusive) {
			this.StartInclusive = startInclusive;
			this.EndExclusive = endExclusive;
		}
		public T StartInclusive { get; set; }
		public T EndExclusive { get; set; }
	}
}
