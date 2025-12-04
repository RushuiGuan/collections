using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Albatross.Collections.Test {
	public class UnionAllTest {

		[Fact]
		public void TestReferenceTypes() {
			var first = new List<string> { "A", "B", "C" };
			var second = new List<string?> { "D", null, "E" };
			var result = first.UnionIfNotNull(second).ToList();
			Assert.Collection(result,
				item => Assert.Equal("A", item),
				item => Assert.Equal("B", item),
				item => Assert.Equal("C", item),
				item => Assert.Equal("D", item),
				item => Assert.Equal("E", item)
			);
		}

		[Fact]
		public void TestValueTypes() {
			var first = new List<int> { 1, 2, 3 };
			var second = new List<int?> { 4, null, 5 };
			var result = first.UnionIfNotNull(second).ToList();
			Assert.Collection(result,
				item => Assert.Equal(1, item),
				item => Assert.Equal(2, item),
				item => Assert.Equal(3, item),
				item => Assert.Equal(4, item),
				item => Assert.Equal(5, item)
			);
		}
	}
}
