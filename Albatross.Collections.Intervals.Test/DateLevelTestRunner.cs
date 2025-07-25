﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Albatross.Collections.Intervals.Test {
	public class DateLevelTestRunner {
		/// <summary>
		/// create date level series with random values.  The first entry will have the start date of seriesStart and the last entry will have the end date of seriesEnd.  
		/// The rest of the entries will always have its start date as the first on the month and end date as the last day of the month.
		/// </summary>
		/// <param name="seriesStart"></param>
		/// <param name="seriesEnd"></param>
		/// <returns></returns>
		public static IEnumerable<DateInterval<int>> CreateMonthlySeries(DateOnly seriesStart, DateOnly seriesEnd, int? value) {
			for (var start = seriesStart; start <= seriesEnd; start = start.AddMonths(1)) {
				var actual_value = value ?? Random.Shared.Next(1, 100);
				var end = start.AddMonths(1).AddDays(-1);
				if (end > seriesEnd) {
					end = seriesEnd;
				}
				var item = new DateInterval<int>(start, end, actual_value);
				yield return item;
			}
		}
		public static IEnumerable<DateInterval<int>> CreateRandomSeries(DateOnly seriesStart, DateOnly seriesEnd, Dictionary<DateOnly, int> dict) {
			DateOnly start, end;
			for (start = seriesStart, end = seriesStart.AddDays(Random.Shared.Next(0, 10)); end <= seriesEnd; start = end.AddDays(1), end = start.AddDays(Random.Shared.Next(0, 10))) {
				var value = Random.Shared.Next(1, 100);
				var item = new DateInterval<int>(start, end, value);
				yield return item;
				for (var date = start; date <= end; date = date.AddDays(1)) {
					dict[date] = value;
				}
			}
			if (end > seriesEnd) {
				var value = Random.Shared.Next(1, 100);
				var item = new DateInterval<int>(start, seriesEnd, value);
				yield return item;
				for (var date = start; date <= seriesEnd; date = date.AddDays(1)) {
					dict[date] = value;
				}
			}
		}

		public DateLevelTestRunner(string seriesStart, string seriesEnd, string start, string end, int value) : this(seriesStart, seriesEnd, start, end, value, value) { }

		public DateLevelTestRunner(string seriesStart, string seriesEnd, string start, string end, int? seriesValue, int srcValue) {
			SeriesStart = DateOnly.Parse(seriesStart, null);
			SeriesEnd = DateOnly.Parse(seriesEnd, null);
			Start = DateOnly.Parse(start, null);
			End = DateOnly.Parse(end, null);
			SourceValue = srcValue;
			SeriesValue = seriesValue;
			List = CreateMonthlySeries(SeriesStart, SeriesEnd, seriesValue).ToList();
			this.SourceItem = new DateInterval<int>(Start, End, SourceValue);
			this.Results = Run();
		}

		public DateOnly SeriesStart { get; }
		public DateOnly SeriesEnd { get; }
		public int? SeriesValue { get; }
		public int RequiredSeriesValue => SeriesValue ?? throw new InvalidOperationException("SeriesValue is random");
		public List<DateInterval<int>> List { get; private set; }

		public DateOnly Start { get; private set; }
		public DateOnly End { get; private set; }
		public int SourceValue { get; private set; }
		public DateInterval<int> SourceItem { get; private set; }

		public List<DateInterval<int>> Results { get; private set; }

		public List<DateInterval<int>> Run() {
			Results = List.Insert(SourceItem, x => x).OrderBy(x => x.StartInclusive).ToList();
			return Results;
		}
	}
}
