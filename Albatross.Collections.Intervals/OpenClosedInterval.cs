using System;

namespace Albatross.Collections.Intervals {
	public interface IOpenClosedInterval<T> where T : IComparable<T> {
		T StartInclusive { get; set; }
		T EndExclusive { get; set; }
	}
	public class OpenClosedInterval<T> : IOpenClosedInterval<T> where T : IComparable<T> {
		public OpenClosedInterval(T startInclusive, T endExclusive) {
			this.StartInclusive = startInclusive;
			this.EndExclusive = endExclusive;
		}
		public T StartInclusive { get; set; }
		public T EndExclusive { get; set; }
	}
}
