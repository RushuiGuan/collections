using Xunit;

namespace Albatross.Collections.Intervals.Test {
	public class TestExtremes {
		[Fact]
		public void TestExtremeValues() {
			var series = new List<IntInterval<int>> { 
				new IntInterval<int>(int.MinValue, 100, 1) ,
				new IntInterval<int>(101, int.MaxValue, 2)
			};
			var src = new IntInterval<int>(102, int.MaxValue, 3);
			series.Insert(src, x => x).ToArray();
		}
	}
}