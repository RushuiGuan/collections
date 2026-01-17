# Albatross Collections

A set of .NET libraries providing utilities for collection operations and interval management.

## Packages

| Package | Description |
|---------|-------------|
| [Albatross.Collections](https://www.nuget.org/packages/Albatross.Collections) | Extension methods for batching, searching, merging, and list manipulation |
| [Albatross.Collections.Intervals](https://www.nuget.org/packages/Albatross.Collections.Intervals) | Continuous, non-overlapping interval management |

## Installation

```bash
dotnet add package Albatross.Collections
dotnet add package Albatross.Collections.Intervals
```

## Albatross.Collections

Extension methods for common collection operations.

### Batching
```csharp
var numbers = Enumerable.Range(1, 10);
foreach (var batch in numbers.Batch(3)) {
    Console.WriteLine(string.Join(", ", batch));
}
// Output: 1, 2, 3 | 4, 5, 6 | 7, 8, 9 | 10
```

### Dictionary Operations
```csharp
var cache = new Dictionary<string, int>();
var value = cache.GetOrAdd("key", () => ComputeExpensiveValue());
```

### Binary Search
```csharp
var sorted = new int[] { 1, 3, 5, 7, 9 };
var result = sorted.BinarySearchFirstValueGreaterOrEqual(6, x => x); // 7
```

### Collection Merging
```csharp
existing.Merge(incoming,
    src => src.Id, dst => dst.Id,
    (src, dst) => { /* matched */ },
    src => { /* added */ },
    dst => { /* removed */ });
```

## Albatross.Collections.Intervals

Manage continuous, non-overlapping intervals for time-series data, date ranges, and range-based lookups.

### Date Intervals
```csharp
var intervals = new List<DateInterval<string>>();

// Insert maintains continuity automatically
intervals = intervals.Insert(
    new DateInterval<string>(new DateOnly(2023, 1, 1), new DateOnly(9999, 12, 31), "Initial"),
    x => x
).ToList();

// Find by point
var result = intervals.Find(new DateOnly(2023, 1, 15));
```

### Update with Auto-Split
```csharp
var intervals = new List<IntInterval<decimal>>();
intervals = intervals.Insert(new IntInterval<decimal>(1, 100, 10.0m), x => x).ToList();

// Updates split intervals automatically
intervals.Update(item => item.Value = 25.0m, x => x, 20, 50);
// Result: [1-19: 10.0], [20-50: 25.0], [51-100: 10.0]
```

### Consolidate Adjacent Intervals
```csharp
var consolidated = intervals.Rebuild((a, b) => a.Value == b.Value).ToList();
```

## License

MIT License - see [LICENSE](https://github.com/RushuiGuan/collections/blob/main/LICENSE)
