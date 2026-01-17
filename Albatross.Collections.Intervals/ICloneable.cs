namespace Albatross.Collections.Intervals {
	/// <summary>
	/// Provides a strongly-typed copy mechanism for interval values.
	/// </summary>
	/// <typeparam name="T">The type to copy.</typeparam>
	public interface ICopy<T> {
		/// <summary>
		/// Creates a copy of the current instance.
		/// </summary>
		/// <returns>A new instance with the same values.</returns>
		public T Copy();
	}
}
