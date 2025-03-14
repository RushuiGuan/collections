﻿using Albatross.Text;
using Albatross.Testing;
using FluentAssertions;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace Albatross.Collections.Test {
	public class TestExtensions {
		[Theory]
		[InlineData("4-6", 5, true, "4,6")]
		[InlineData("1-o10", 4, false, "1,3,5,7,9")]
		[InlineData("1,1,2,3", 1, true, "1,2,3")]
		public void TryGetOneAndRemove(string array, int target, bool expected, string expected_text) {
			var list = array.IntArray().ToList();
			var old_length = list.Count;
			var has = list.TryGetOneAndRemove(x => x == target, out var result);
			has.Should().Be(expected);
			list.AsString().Should().Be(expected_text);
			if (expected) {
				result.Should().Be(target);
			}
		}

		[Fact]
		public void TestRemoveAny() {
			var list = "1,2,3,4,5".IntArray().ToList();
			var old_length = list.Count;
			var removed = list.RemoveAny(x => x == 3 || x == 4);
			list.AsString().Should().Be("1,2,5");
			removed.AsString().Should().Be("4,3");
		}

		[Fact]
		public void TestRemoveAny_FromRear() {
			var list = "1,2,3,4,5".IntArray().ToList();
			var old_length = list.Count;
			var removed = list.RemoveAny_FromRear(x => x == 4 || x == 5);
			list.AsString().Should().Be("1,2,3");
			removed.AsString().Should().Be("5,4");
		}

		[Fact]
		public void TestRemoveAny_WithNewList() {
			var list = "1,2,3,4,5".IntArray().ToList();
			var old_length = list.Count;
			var removed = list.RemoveAny_WithNewList(x => x == 4 || x == 5);
			list.AsString().Should().Be("1,2,3");
			removed.AsString().Should().Be("4,5");
		}

		[Theory]
		[InlineData("1-10", 1)]
		[InlineData("1-10", 2)]
		[InlineData("1-10", 3)]
		[InlineData("1-10", 4)]
		[InlineData("1-10", 5)]
		[InlineData("1-10", 6)]
		[InlineData("1-10", 7)]
		[InlineData("1-10", 8)]
		[InlineData("1-10", 9)]
		[InlineData("1-10", 10)]
		[InlineData("1-10", 11)]
		[InlineData("1-10", 12)]
		[InlineData("1-10", 13)]
		public void TestBatch(string arrayText, int size) {
			var array = arrayText.IntArray();
			var result = new StringWriter();
			var batches = array.Batch(size).ToArray();
			result.WriteItems(batches, ".", (w, items) => w.WriteItems(items, "."));

			var expected = new StringWriter();
			expected.WriteItems(array, ".", null);
			
			Assert.Equal(expected.ToString(), result.ToString());
		}
	}
}