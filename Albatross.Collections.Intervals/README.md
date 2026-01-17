# Albatross.Collections.Intervals

A .NET library for managing continuous, non-overlapping intervals. Useful for time-series data, date ranges, and scenarios where data spans ranges rather than single values.

## Installation

```bash
dotnet add package Albatross.Collections.Intervals
```

## Quick Start

### Date Intervals
```csharp
using Albatross.Collections.Intervals;

var intervals = new List<DateInterval<string>>();

// Insert creates continuous series - gaps are not allowed
intervals = intervals.Insert(
    new DateInterval<string>(new DateOnly(2023, 1, 1), new DateOnly(9999, 12, 31), "Initial"),
    x => x
).ToList();

// Second insert adjusts the first interval's end date automatically
intervals = intervals.Insert(
    new DateInterval<string>(new DateOnly(2023, 2, 1), new DateOnly(9999, 12, 31), "February"),
    x => x
).ToList();

// Find by point
var result = intervals.Find(new DateOnly(2023, 1, 15));
Console.WriteLine(result?.Value); // "Initial"

// Find by range
var rangeResults = intervals.Find(new DateOnly(2023, 1, 20), new DateOnly(2023, 2, 10));
```

### Integer Intervals with Update
```csharp
var intervals = new List<IntInterval<decimal>>();

// Create base interval
intervals = intervals.Insert(new IntInterval<decimal>(1, 100, 10.0m), x => x).ToList();

// Update splits intervals automatically
intervals.Update(
    item => item.Value = 25.0m,  // modification
    x => x,                       // clone function
    20, 50                        // range to update
);
// Result: [1-19: 10.0], [20-50: 25.0], [51-100: 10.0]
```

### Merging Adjacent Intervals
```csharp
var intervals = new List<IntInterval<int>> {
    new(1, 10, 100),
    new(11, 20, 100),  // same value, adjacent
    new(21, 30, 200)
};

// Rebuild consolidates adjacent intervals with equal values
var consolidated = intervals.Rebuild((a, b) => a.Value == b.Value).ToList();
// Result: [1-20: 100], [21-30: 200]
```

### Join Two Series
```csharp
var prices = new List<DateInterval<decimal>> { /* price history */ };
var quantities = new List<DateInterval<int>> { /* quantity history */ };

var combined = prices.Join<DateInterval<decimal>, DateInterval<int>, DateInterval<decimal>, DateOnly>(
    quantities,
    (price, qty) => new DateInterval<decimal>(default, default, price.Value * qty.Value)
);
```

### Validation
```csharp
// Verify series is continuous and non-overlapping
bool isValid = intervals.Verify(throwException: false);
```

## API Reference

| Method | Description |
|--------|-------------|
| `Insert<T,K>(item, clone)` | Add interval maintaining continuity, auto-merges equal adjacent values |
| `Update<T,K>(modify, clone, start, end)` | Update values in range, auto-splits intervals |
| `Find<T,K>(key)` | Find interval containing point |
| `Find<T,K>(start, end)` | Find all intervals overlapping range |
| `FindRequired<T,K>(key)` | Find or throw if not found |
| `TrimStart<T,K>(newStart)` | Remove data before new start |
| `TrimEnd<T,K>(newEnd)` | Remove data after new end |
| `Rebuild<T,K>(isEqual)` | Consolidate adjacent intervals with matching values |
| `Merge<T,K>()` | Merge overlapping/adjacent intervals |
| `Join<TLeft,TRight,TResult,TKey>(...)` | Combine two interval series |
| `Verify<T,K>(throwException)` | Validate continuity and non-overlap |
| `IsValid<T>()` | Check if interval start <= end |

## Built-in Types

| Type | Key Type | Description |
|------|----------|-------------|
| `DateInterval<V>` | `DateOnly` | Date-based intervals |
| `IntInterval<V>` | `int` | Integer-based intervals |

## Custom Interval Types

Implement `IClosedInterval<K>` or `IClosedInterval<K,V>`:

```csharp
public class MyInterval : IClosedInterval<int, string> {
    public int StartInclusive { get; set; }
    public int EndInclusive { get; set; }
    public string Value { get; set; }

    public static int Next(int value) => value + 1;
    public static int Previous(int value) => value - 1;
}
```

## License

MIT License - see [LICENSE](../LICENSE)
