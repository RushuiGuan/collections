namespace Albatross.Collections.Intervals {
	public class Joiner<T1, T2, T3, K> where K : IComparable<K> where T1 : IClosedInterval<K> where T2 : IClosedInterval<K> where T3 : IClosedInterval<K> {
		public K StartInclusive { get; set; }
		public K EndInclusive { get; set; }

		Stack<T1> stack1 = new Stack<T1>();
		Stack<T2> stack2 = new Stack<T2>();
		List<T3> result = new List<T3>();
		private readonly Func<T1, T2, T3> func;

		public Joiner(IEnumerable<T1> series1, IEnumerable<T2> series2, Func<T1, T2, T3> func) {
			stack1 = new Stack<T1>(series1.OrderByDescending(x => x.StartInclusive));
			stack2 = new Stack<T2>(series2.OrderByDescending(x => x.StartInclusive));
			this.func = func;
		}


		public IEnumerable<T3> Run() {

			if (stack1.Count == 0 || stack2.Count == 0) {
				yield break;
			} else if (stack1.Count == 0 || stack2.Count == 0) {
				throw new ArgumentException("Both series must have at least one interval.");
			} else {
				var item1 = stack1.Pop();
				var item2 = stack2.Pop();
				if (item1.StartInclusive.CompareTo(item2.StartInclusive) != 0) {
					throw new ArgumentException("Intervals must start at the same point.");
				}
				var item3 = func(item1, item2);
				item3.EndInclusive = item1.EndInclusive.IsLessThanOrEqualTo(item2.EndInclusive) ? item1.EndInclusive : item2.EndInclusive;
				yield return item3;
			}
		}

		IClosedInterval<K>? NextNode() {
			if (queue1.Count == 0 && queue2.Count == 0) {
				return null;
			}
			if (queue1.Count == 0) {
				return queue2.Dequeue();
			} else if (queue2.Count == 0) {
				return queue1.Dequeue();
			} else {
				var item1 = queue1.Peek();
				var item2 = queue2.Peek();
				if (item1.StartInclusive.IsLessThanOrEqualTo(item2.StartInclusive)) {
					return queue1.Dequeue();
				} else {
					return queue2.Dequeue();
				}
			}
		}
	}
}