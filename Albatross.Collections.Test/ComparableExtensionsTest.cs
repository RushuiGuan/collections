using Albatross.Collections.Intervals;
using System.Numerics;
using Xunit;

namespace Albatross.Collections.Test {
	public class ComparableExtensionsTest {
		[Theory]
		[InlineData(1, 1, false)]
		[InlineData(1, 0, true)]
		[InlineData(0, 1, false)]
		public void TestIsGreaterThan(int left, int right, bool expected) {
			var result = left.IsGreaterThan(right);
			Assert.Equal(expected, result);
		}
		[Theory]
		[InlineData(1, 0, true)]
		[InlineData(0, 0, true)]
		[InlineData(0, 1, false)]
		public void TestIsGreaterThanOrEqualTo(int left, int right, bool expected) {
			var result = left.IsGreaterThanOrEqualTo(right);
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData(0, 1, true)]
		[InlineData(1, 0, false)]
		[InlineData(0, 0, false)]
		public void TestIsLessThan(int left, int right, bool expected) {
			var result = left.IsLessThan(right);
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData(0, 1, true)]
		[InlineData(0, 0, true)]
		[InlineData(1, 0, false)]
		public void TestIsLessThanOrEqualTo(int left, int right, bool expected) {
			var result = left.IsLessThanOrEqualTo(right);
			Assert.Equal(expected, result);
		}
		[Theory]
		[InlineData(1, 0, 2, true)]
		[InlineData(0, 0, 2, true)]
		[InlineData(2, 0, 2, true)]
		[InlineData(3, 0, 2, false)]
		[InlineData(-1, 0, 2, false)]
		public void TestIsBetweenInclusive(int value, int start, int end, bool expected) {
			var result = value.IsBetweenInclusive(start, end);
			Assert.Equal(expected, result);
		}
		[Theory]
		[InlineData(1, 0, 2, true)]
		[InlineData(0, 0, 2, false)]
		[InlineData(2, 0, 2, false)]
		[InlineData(3, 0, 2, false)]
		[InlineData(-1, 0, 2, false)]
		public void TestIsBetweenExclusive(int value, int start, int end, bool expected) {
			var result = value.IsBetweenExclusive(start, end);
			Assert.Equal(expected, result);
		}
		[Theory]
		[InlineData(1, 0, 2, true)]
		[InlineData(0, 0, 2, true)]
		[InlineData(2, 0, 2, false)]
		[InlineData(3, 0, 2, false)]
		[InlineData(-1, 0, 2, false)]
		public void TestIsBetween_OpenClosed(int value, int start, int end, bool expected) {
			var result = value.IsBetween_OpenClosed(start, end);
			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData(1, 0, 2, true)]
		[InlineData(0, 0, 2, false)]
		[InlineData(2, 0, 2, true)]
		[InlineData(3, 0, 2, false)]
		[InlineData(-1, 0, 2, false)]
		public void TestIsBetween_ClosedOpen(int value, int start, int end, bool expected) {
			var result = value.IsBetween_ClosedOpen(start, end);
			Assert.Equal(expected, result);
		}
	}
}
