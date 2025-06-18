using FluentAssertions;
using Xunit;

namespace Albatross.Collections.Intervals.Test {
	public record class TestValue {
		public int Number1 { get; set; }
		public int Number2 { get; set; }
	}

	public class CloseIntervalUpdateStressTester {
		Dictionary<int, TestValue> dict = new();
		List<IntInterval<TestValue>> intervals = new();

		public CloseIntervalUpdateStressTester(int min, int max) {
			var first = new IntInterval<TestValue>(min, max, new TestValue { Number1 = 1, Number2 = 2, });
			intervals.Add(first);
			for (int i = min; i <= max; i++) {
				dict[i] = first.Value with { };
			}

			Min = min;
			Max = max;
		}

		public int Min { get; private set; }
		public int Max { get; private set; }

		public void TrimStart(int value) {
			intervals = intervals.TrimStart(value).ToList();
			Min = value;
		}

		public void TrimEnd(int value) {
			intervals = intervals.TrimEnd(value).ToList();
			Max = value;
		}

		public void RandomChange(Action<TestValue> action) {
			var start = Random.Shared.Next(Min, Max);
			var end = Random.Shared.Next(start, Max);
			intervals.Update(x => action(x.Value), start, end, v => v with { });
			for (int i = start; i <= end; i++) {
				action(dict[i]);
			}
		}
		public void RandomChange1(int value) {
			RandomChange(v => v.Number1 = value);
		}
		public void RandomChange2(int value) => RandomChange(v => v.Number2 = value);

		public void Verify() {
			var sorted = intervals.OrderBy(x => x.StartInclusive).ToList();
			sorted.First().StartInclusive.Should().Be(Min);
			sorted.Last().EndInclusive.Should().Be(Max);
			intervals.Verify<IntInterval<TestValue>, int>(true);

			for (int i = Min; i <= Max; i++) {
				var value = intervals.Find(i)?.Value;
				Assert.NotNull(value);
				Assert.Equal(dict[i].Number1, value.Number1);
				Assert.Equal(dict[i].Number2, value.Number2);
			}

			var rebuild = intervals.Rebuild<IntInterval<TestValue>, int>((a, b) => a.Value == b.Value).ToList();
			for(int i = Min; i <= Max; i++) {
				var value = rebuild.Find(i)?.Value;
				Assert.NotNull(value);
				Assert.Equal(dict[i].Number1, value.Number1);
				Assert.Equal(dict[i].Number2, value.Number2);
			}
			rebuild.Verify<IntInterval<TestValue>, int>(true);
			rebuild.First().StartInclusive.Should().Be(Min);
			rebuild.Last().EndInclusive.Should().Be(Max);
		}

		public void Verify(int start, int end) {
			var result = intervals.Find(start, end).OrderBy(x => x.StartInclusive).ToList();
			result.First().StartInclusive.IsLessThanOrEqualTo(start);
			result.First().EndInclusive.IsGreaterThanOrEqualTo(start);
			result.Last().EndInclusive.IsGreaterThanOrEqualTo(end);
			result.Last().StartInclusive.IsLessThanOrEqualTo(end);
			for(int i = start; i <= end; i++) {
				var value = result.Find(i)?.Value;
				Assert.NotNull(value);
				Assert.Equal(dict[i].Number1, value.Number1);
				Assert.Equal(dict[i].Number2, value.Number2);
			}
		}
	}
}
