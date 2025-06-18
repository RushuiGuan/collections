using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Albatross.Collections.Intervals.Test {
	public class TestClosedIntervalExtentions {
		[Fact]
		public void TestIsValid() {
			var interval = new IntInterval<int>(1, 10, 100);
			Assert.True(interval.IsValid());

			interval = new IntInterval<int>(1, 1, 1000);
			Assert.True(interval.IsValid());

			interval = new IntInterval<int>(10, 1, 100);
			Assert.False(interval.IsValid());
		}

		[Theory]
		[InlineData(100, 10)]
		[InlineData(1000, 100)]
		public void TestTrimStart(int max, int changeCount) {
			var tester = new CloseIntervalUpdateStressTester(1, max);
			for (int i = 0; i < changeCount; i++) {
				tester.RandomChange1(Random.Shared.Next());
				tester.RandomChange2(Random.Shared.Next());
			}

			tester.TrimStart(max / 2);
			tester.Verify();
		}

		[Theory]
		[InlineData(100, 10)]
		[InlineData(1000, 100)]
		public void TestTrimEnd(int max, int changeCount) {
			var tester = new CloseIntervalUpdateStressTester(1, max);
			for (int i = 0; i < changeCount; i++) {
				tester.RandomChange1(Random.Shared.Next());
				tester.RandomChange2(Random.Shared.Next());
			}

			tester.TrimEnd(max / 2);
			tester.Verify();
		}


		[Theory]
		[InlineData(100, 10)]
		[InlineData(1000, 100)]
		public void TestFindArray(int max, int changeCount) {
			var tester = new CloseIntervalUpdateStressTester(1, max);
			for (int i = 0; i < changeCount; i++) {
				tester.RandomChange1(Random.Shared.Next());
				tester.RandomChange2(Random.Shared.Next());
			}
			tester.Verify(max / 3, max / 3 * 2);
		}
	}
}
