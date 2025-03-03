using System;

namespace Albatross.Collections.Intervals {
	public interface IOpenInterval<T> where T : IComparable<T> {
		T StartExclusive { get; set; }
		T EndExclusive { get; set; }
	}
	public class OpenInterval<T> : IOpenInterval<T> where T : IComparable<T> {
		public OpenInterval(T startExclusive, T endExclusive) {
			this.StartExclusive = startExclusive;
			this.EndExclusive = endExclusive;
		}
		public T StartExclusive { get; set; }
		public T EndExclusive { get; set; }
	}
}