using FluentAssertions;
using Xunit;

namespace Albatross.Collections.Intervals.Test {
	
	public class TestUpdateClosedIntervals {
		[Fact]
		public void UpdateWithNoRecord() {
			var list = new List<IntInterval<TestValue>>();
			list.Update(x => x.Value = new TestValue { Number1 = 1 }, 0, 10, x => x with { });
			Assert.Empty(list);
		}

		[Fact]
		public void UpdateAll() {
			var list = new List<IntInterval<TestValue>> {
				new IntInterval<TestValue> (0, 10, new TestValue { Number1 = 1, Number2 = 2 }),
			};
			list.Update(x => x.Value.Number1 = 2, int.MinValue, int.MaxValue, x => x with { });
			for (int i = 0; i < 10; i++) {
				var value = list.Find(i)?.Value;
				Assert.NotNull(value);
				Assert.Equal(2, value.Number1);
				Assert.Equal(2, value.Number2);
			}
		}

		[Fact]
		public void UpdateFrontOnly() {
			var list = new List<IntInterval<TestValue>> {
				new IntInterval<TestValue> (0, 10, new TestValue { Number1 = 1,  Number2 = 2 }),
			};
			list.Update(x => x.Value.Number1 = 2, int.MinValue, 5, x => x with { });
			for (int i = 0; i <= 5; i++) {
				var value = list.Find(i)?.Value;
				Assert.NotNull(value);
				Assert.Equal(2, value.Number1);
				Assert.Equal(2, value.Number2);
			}
			for (int i = 6; i < 10; i++) {
				var value = list.Find(i)?.Value;
				Assert.NotNull(value);
				Assert.Equal(1, value.Number1);
				Assert.Equal(2, value.Number2);
			}
		}

		[Fact]
		public void UpdateBackOnly() {
			var list = new List<IntInterval<TestValue>> {
				new IntInterval<TestValue> (0, 10, new TestValue { Number1 = 1, Number2 = 2 }),
			};
			list.Update(x => x.Value.Number1 = 2, 6, int.MaxValue, x => x with { });
			for (int i = 0; i <= 5; i++) {
				var value = list.Find(i)?.Value;
				Assert.NotNull(value);
				Assert.Equal(1, value.Number1);
				Assert.Equal(2, value.Number2);
			}
			for (int i = 6; i < 10; i++) {
				var value = list.Find(i)?.Value;
				Assert.NotNull(value);
				Assert.Equal(2, value.Number1);
				Assert.Equal(2, value.Number2);
			}
		}

		[Theory]
		//[InlineData(1000, 100)]
		//[InlineData(1000, 10)]
		//[InlineData(10000, 10)]
		//[InlineData(10000, 50)]
		[InlineData(10, 2)]
		public void StressTest(int max, int numberOfChanges) {
			var tester = new CloseIntervalUpdateStressTester(0, max);

			for (int i = 0; i < numberOfChanges; i++) {
				tester.RandomChange1(Random.Shared.Next());
				tester.RandomChange2(Random.Shared.Next());
			}
			tester.Verify();
		}
	}
}

