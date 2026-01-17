namespace Albatross.Collections.Intervals {
	/// <summary>
	/// Exception thrown when interval operations fail due to invalid data or constraint violations such as gaps or overlaps.
	/// </summary>
	public class IntervalException : Exception {
		public IntervalException(string msg) : base(msg) { }
	}
}
