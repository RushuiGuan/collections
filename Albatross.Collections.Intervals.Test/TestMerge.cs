using Xunit;

namespace Albatross.Collections.Intervals.Test {
	public class TestMerge {
		[Fact]
		public void TestMergeAdjacentIntervals() {
			var intervals = new List<IntInterval<string>> {
				new IntInterval<string>(50, 99, "A"),
				new IntInterval<string>(0, 49, "A"),
				new IntInterval<string>(100, 149, "A"),
			};
			var result = intervals.Merge<IntInterval<string>, int>().ToList();
			Assert.Single(result);
			Assert.Equal(0, result.First().StartInclusive);
			Assert.Equal(149, result.First().EndInclusive);
			Assert.Equal("A", result.First().Value);
		}

		[Fact]
		public void TestMergeOverlappingIntervals() {
			var intervals = new List<IntInterval<string>> {
				new IntInterval<string>(0, 49, "A"),
				new IntInterval<string>(40, 60, "A"),
				new IntInterval<string>(55, 70, "A"),
			};
			var result = intervals.Merge<IntInterval<string>, int>().ToList();
			Assert.Single(result);
			Assert.Equal(0, result.First().StartInclusive);
			Assert.Equal(70, result.First().EndInclusive);
			Assert.Equal("A", result.First().Value);
		}

		[Fact]
		public void TestMergeDiscontinuousIntervals() {
			var intervals = new List<IntInterval<string>> {
				new IntInterval<string>(0, 49, "A"),
				new IntInterval<string>(51, 99, "A"), // gap at 50
				new IntInterval<string>(120, 149, "A"), // gap between 99 and 120
			};
			var result = intervals.Merge<IntInterval<string>, int>().ToList();
			Assert.Equal(3, result.Count);
			Assert.Equal(0, result[0].StartInclusive);
			Assert.Equal(49, result[0].EndInclusive);
			Assert.Equal("A", result[0].Value);

			Assert.Equal(51, result[1].StartInclusive);
			Assert.Equal(99, result[1].EndInclusive);
			Assert.Equal("A", result[1].Value);

			Assert.Equal(120, result[2].StartInclusive);
			Assert.Equal(149, result[2].EndInclusive);
			Assert.Equal("A", result[2].Value);
		}
	}
}
