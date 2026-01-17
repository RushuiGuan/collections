using System;

namespace Albatross.Collections.Intervals {
	/// <summary>
	/// Represents an open interval (start, end) where both boundaries are exclusive.
	/// </summary>
	/// <typeparam name="T">The type of the interval boundaries.</typeparam>
	public interface IOpenInterval<T> where T : IComparable<T> {
		T StartExclusive { get; set; }
		T EndExclusive { get; set; }
	}

	/// <summary>
	/// A basic implementation of an open interval (start, end) with exclusive boundaries.
	/// </summary>
	/// <typeparam name="T">The type of the interval boundaries.</typeparam>
	public class OpenInterval<T> : IOpenInterval<T> where T : IComparable<T> {
		public OpenInterval(T startExclusive, T endExclusive) {
			this.StartExclusive = startExclusive;
			this.EndExclusive = endExclusive;
		}
		public T StartExclusive { get; set; }
		public T EndExclusive { get; set; }
	}
}
