using Xunit;

namespace Albatross.Collections.Intervals.Test {
	public class TestCloseIntervalJoin {
		[Fact]
		public void TestLeftOverlap() {
			var left = new IntInterval<int>[] {
				new IntInterval<int>(0, 10, 100)
			};
			var right = new IntInterval<int>[] {
				new IntInterval<int>(5, 15, 200)
			};
			var result = left.Join<IntInterval<int>, IntInterval<int>, IntInterval<int>, int>(right, (a, b) => new IntInterval<int>(0, 0, a.Value + b.Value)).ToArray();
			Assert.Single(result);
			Assert.Equal(300, result[0].Value);
			Assert.Equal(5, result[0].StartInclusive);
			Assert.Equal(10, result[0].EndInclusive);
		}

		[Fact]
		public void TestRightOverlap() {
			var left = new IntInterval<int>[] {
				new IntInterval<int>(5, 15, 200)
			};
			var right = new IntInterval<int>[] {
				new IntInterval<int>(0, 10, 100)
			};
			var result = left.Join<IntInterval<int>, IntInterval<int>, IntInterval<int>, int>(right, (a, b) => new IntInterval<int>(0, 0, a.Value + b.Value)).ToArray();
			Assert.Single(result);
			Assert.Equal(300, result[0].Value);
			Assert.Equal(5, result[0].StartInclusive);
			Assert.Equal(10, result[0].EndInclusive);
		}

		[Fact]
		public void TestEqualRange() {
			var left = new IntInterval<int>[] {
				new IntInterval<int>(5, 15, 200)
			};
			var right = new IntInterval<int>[] {
				new IntInterval<int>(5, 15, 100)
			};
			var result = left.Join<IntInterval<int>, IntInterval<int>, IntInterval<int>, int>(right, (a, b) => new IntInterval<int>(0, 0, a.Value + b.Value)).ToArray();
			Assert.Single(result);
			Assert.Equal(300, result[0].Value);
			Assert.Equal(5, result[0].StartInclusive);
			Assert.Equal(15, result[0].EndInclusive);
		}

		[Fact]
		public void TestFullyOverlap1() {
			var left = new IntInterval<int>[] {
				new IntInterval<int>(0, 15, 200)
			};
			var right = new IntInterval<int>[] {
				new IntInterval<int>(5, 10, 100)
			};
			var result = left.Join<IntInterval<int>, IntInterval<int>, IntInterval<int>, int>(right, (a, b) => new IntInterval<int>(0, 0, a.Value + b.Value)).ToArray();
			Assert.Single(result);
			Assert.Equal(300, result[0].Value);
			Assert.Equal(5, result[0].StartInclusive);
			Assert.Equal(10, result[0].EndInclusive);
		}

		[Fact]
		public void TestFullyOverlap2() {
			var left = new IntInterval<int>[] {
				new IntInterval<int>(5, 10, 100)
			};
			var right = new IntInterval<int>[] {
				new IntInterval<int>(0, 15, 200)
			};
			var result = left.Join<IntInterval<int>, IntInterval<int>, IntInterval<int>, int>(right, (a, b) => new IntInterval<int>(0, 0, a.Value + b.Value)).ToArray();
			Assert.Single(result);
			Assert.Equal(300, result[0].Value);
			Assert.Equal(5, result[0].StartInclusive);
			Assert.Equal(10, result[0].EndInclusive);
		}

		IEnumerable<IntInterval<int>> Generate(int first, int last) {
			var start = first;
			do {
				var end = Random.Shared.Next(start, last);
				yield return new IntInterval<int>(start, end, Random.Shared.Next(0, 1000));
				start = end + 1;
			} while (start <= last);
		}

		[Theory]
		[InlineData(500, 600, 100, 1000)] // fully overlap
		[InlineData(100, 1000, 500, 600)] // fully overlap
		[InlineData(100, 200, 100, 200)] // equal range
		[InlineData(100, 200, 300, 400)] // no overlap
		[InlineData(100, 200, 200, 300)] // single point overlap
		public void StressTest(int leftStart, int leftEnd, int rightStart, int rightEnd) {
			var left = Generate(leftStart, leftEnd).ToArray();
			var right = Generate(rightStart, rightEnd).ToArray();
			var result = left.Join<IntInterval<int>, IntInterval<int>, IntInterval<int>, int>(right, (a, b) => new IntInterval<int>(0, 0, a.Value + b.Value)).ToArray();
			if (leftEnd < rightStart || rightEnd < leftStart) {
				Assert.Empty(result);
			} else {
				Assert.NotEmpty(result);
				var start = result.First().StartInclusive;
				var end = result.Last().EndInclusive;
				Assert.Equal(int.Max(leftStart, rightStart), start);
				Assert.Equal(int.Min(leftEnd, rightEnd), end);
				result.Verify<IntInterval<int>, int>(true);
				for (var i = start; i <= end; i++) {
					var item = result.Find(i);
					var leftItem = left.Find(i);
					var rightItem = right.Find(i);
					Assert.Equal(leftItem?.Value + rightItem?.Value, item?.Value);
				}
			}
		}
	}
}