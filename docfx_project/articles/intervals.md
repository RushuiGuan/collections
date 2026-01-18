# Albatross.Collections.Intervals

A .NET library for managing continuous, non-overlapping intervals. Designed for time-series data, date ranges, pricing history, and any scenario where data spans ranges rather than single values.

## Installation

```bash
dotnet add package Albatross.Collections.Intervals
```

## Core Concepts

### Continuous Series

The library enforces that interval series remain **continuous** (no gaps) and **non-overlapping**. When you insert a new interval, the library automatically adjusts existing intervals to maintain these invariants.

### Interval Types

- **Closed Intervals**: Both start and end are inclusive `[start, end]`
- Intervals require `Next()` and `Previous()` static methods to determine adjacent values

## Built-in Interval Types

### DateInterval

For date-based intervals using `DateOnly`.

```csharp
var interval = new DateInterval<string>(
    new DateOnly(2023, 1, 1),   // Start (inclusive)
    new DateOnly(2023, 12, 31), // End (inclusive)
    "2023 Data"                  // Value
);
```

### IntInterval

For integer-based intervals.

```csharp
var interval = new IntInterval<decimal>(
    1,      // Start (inclusive)
    100,    // End (inclusive)
    99.99m  // Value
);
```

## Insert Operations

The `Insert` method adds intervals while maintaining continuity and merging adjacent intervals with equal values.

### Basic Insert

```csharp
var intervals = new List<DateInterval<string>>();

// First insert creates the initial interval
intervals = intervals.Insert(
    new DateInterval<string>(new DateOnly(2023, 1, 1), new DateOnly(9999, 12, 31), "Initial"),
    x => x  // Clone function
).ToList();

// Second insert adjusts the first interval's end date
intervals = intervals.Insert(
    new DateInterval<string>(new DateOnly(2023, 6, 1), new DateOnly(9999, 12, 31), "June onwards"),
    x => x
).ToList();

// Result:
// [2023-01-01 to 2023-05-31]: "Initial"
// [2023-06-01 to 9999-12-31]: "June onwards"
```

### Auto-Merging Equal Values

When inserting an interval with the same value as an adjacent interval, they merge automatically.

```csharp
var intervals = new List<IntInterval<int>>();

intervals = intervals.Insert(new IntInterval<int>(1, 50, 100), x => x).ToList();
intervals = intervals.Insert(new IntInterval<int>(51, 100, 100), x => x).ToList();

// Result: Single interval [1-100]: 100 (merged because values are equal)
```

### Insert with Clone Function

The clone function creates copies when splitting intervals:

```csharp
intervals = intervals.Insert(
    newInterval,
    original => new DateInterval<MyClass>(
        original.StartInclusive,
        original.EndInclusive,
        original.Value.Clone()  // Deep clone the value
    )
).ToList();
```

### Insert with Equality Function

Control when intervals should merge:

```csharp
intervals = intervals.Insert(
    newInterval,
    isEqual: (a, b) => a.Value.Category == b.Value.Category,
    clone: x => x
).ToList();
```

## Update Operations

The `Update` method modifies values within a range, automatically splitting intervals as needed.

### Basic Update

```csharp
var intervals = new List<IntInterval<decimal>>();
intervals = intervals.Insert(new IntInterval<decimal>(1, 100, 10.0m), x => x).ToList();

// Update values in range [20, 50]
intervals.Update(
    item => item.Value = 25.0m,  // Modification action
    x => x,                       // Clone function
    20, 50                        // Range
);

// Result:
// [1-19]: 10.0
// [20-50]: 25.0
// [51-100]: 10.0
```

### Update Scenarios

The update operation handles various overlap scenarios:

```csharp
// Original: [1-100]: 10.0

// 1. Update completely within: Update(30, 40) splits into 3 intervals
// 2. Update overlaps start: Update(1, 30) creates 2 intervals
// 3. Update overlaps end: Update(70, 100) creates 2 intervals
// 4. Update covers entire interval: Update(1, 100) modifies in place
```

## Find Operations

### Find by Point

```csharp
var intervals = new List<DateInterval<string>> { /* ... */ };

// Find interval containing a specific date
var result = intervals.Find(new DateOnly(2023, 6, 15));
if (result != null) {
    Console.WriteLine($"Value: {result.Value}");
}

// Throws if not found
var required = intervals.FindRequired(new DateOnly(2023, 6, 15));
```

### Find by Range

```csharp
// Find all intervals overlapping a date range
var overlapping = intervals.Find(
    new DateOnly(2023, 1, 1),
    new DateOnly(2023, 6, 30)
);

foreach (var interval in overlapping) {
    Console.WriteLine($"{interval.StartInclusive} - {interval.EndInclusive}: {interval.Value}");
}
```

## Trim Operations

Adjust interval boundaries by removing data outside a range.

### TrimStart

Remove all data before a new start date.

```csharp
var intervals = new List<DateInterval<int>> {
    new(new DateOnly(2023, 1, 1), new DateOnly(2023, 6, 30), 100),
    new(new DateOnly(2023, 7, 1), new DateOnly(2023, 12, 31), 200)
};

var trimmed = intervals.TrimStart(new DateOnly(2023, 3, 1)).ToList();

// Result:
// [2023-03-01 to 2023-06-30]: 100  (start adjusted)
// [2023-07-01 to 2023-12-31]: 200  (unchanged)
```

### TrimEnd

Remove all data after a new end date.

```csharp
var trimmed = intervals.TrimEnd(new DateOnly(2023, 9, 30)).ToList();

// Result:
// [2023-01-01 to 2023-06-30]: 100  (unchanged)
// [2023-07-01 to 2023-09-30]: 200  (end adjusted)
```

## Rebuild Operations

Consolidate adjacent intervals with matching values.

```csharp
var intervals = new List<IntInterval<int>> {
    new(1, 10, 100),
    new(11, 20, 100),   // Same value as previous
    new(21, 30, 100),   // Same value as previous
    new(31, 40, 200)    // Different value
};

var consolidated = intervals.Rebuild((a, b) => a.Value == b.Value).ToList();

// Result:
// [1-30]: 100   (three intervals merged)
// [31-40]: 200
```

## Merge Operations

Merge overlapping or adjacent intervals into single intervals, assuming all intervals represent the same logical value. The input does not need to be continuous, and the output will not be continuous if there are gaps. Intervals that do not overlap or touch remain separate.

```csharp
var intervals = new List<IntInterval<int>> {
    new(1, 10, 100),
    new(5, 15, 100),    // Overlaps with first - will be merged
    new(20, 30, 100)    // Gap from previous - stays separate
};

var merged = intervals.Merge<IntInterval<int>, int>().ToList();

// Result:
// [1-15]: 100   (first two merged due to overlap)
// [20-30]: 100  (separate - gap preserved)
```

## Join Operations

Combine two interval series, creating new intervals where they overlap.

```csharp
var prices = new List<DateInterval<decimal>> {
    new(new DateOnly(2023, 1, 1), new DateOnly(2023, 6, 30), 10.00m),
    new(new DateOnly(2023, 7, 1), new DateOnly(2023, 12, 31), 12.00m)
};

var quantities = new List<DateInterval<int>> {
    new(new DateOnly(2023, 1, 1), new DateOnly(2023, 3, 31), 100),
    new(new DateOnly(2023, 4, 1), new DateOnly(2023, 12, 31), 150)
};

var revenue = prices.Join<DateInterval<decimal>, DateInterval<int>, DateInterval<decimal>, DateOnly>(
    quantities,
    (price, qty) => new DateInterval<decimal>(default, default, price.Value * qty.Value)
).ToList();

// Result:
// [2023-01-01 to 2023-03-31]: 1000.00  (10 * 100)
// [2023-04-01 to 2023-06-30]: 1500.00  (10 * 150)
// [2023-07-01 to 2023-12-31]: 1800.00  (12 * 150)
```

## Validation

### Verify

Check that a series maintains continuity and non-overlap invariants.

```csharp
var intervals = new List<DateInterval<int>> { /* ... */ };

// Returns false if invalid
bool isValid = intervals.Verify(throwException: false);

// Throws IntervalException with details if invalid
try {
    intervals.Verify(throwException: true);
} catch (IntervalException ex) {
    Console.WriteLine(ex.Message);
}
```

### IsValid

Check if a single interval has valid boundaries (start <= end).

```csharp
var interval = new IntInterval<int>(10, 5, 100);  // Invalid: start > end
bool valid = interval.IsValid();  // false
```

## Custom Interval Types

Implement `IClosedInterval<K>` for key-only intervals or `IClosedInterval<K,V>` for intervals with values.

```csharp
public class MonthInterval<V> : IClosedInterval<int, V> where V : IEquatable<V> {
    public int StartInclusive { get; set; }  // e.g., 202301
    public int EndInclusive { get; set; }    // e.g., 202312
    public V Value { get; set; }

    // Required: Define how to get next/previous values
    public static int Next(int value) {
        var year = value / 100;
        var month = value % 100;
        if (month == 12) {
            return (year + 1) * 100 + 1;
        }
        return value + 1;
    }

    public static int Previous(int value) {
        var year = value / 100;
        var month = value % 100;
        if (month == 1) {
            return (year - 1) * 100 + 12;
        }
        return value - 1;
    }

    public MonthInterval() { }

    public MonthInterval(int start, int end, V value) {
        StartInclusive = start;
        EndInclusive = end;
        Value = value;
    }
}
```

Usage:

```csharp
var intervals = new List<MonthInterval<string>>();
intervals = intervals.Insert(
    new MonthInterval<string>(202301, 999912, "Initial"),
    x => new MonthInterval<string>(x.StartInclusive, x.EndInclusive, x.Value)
).ToList();
```

## API Reference

| Method | Description |
|--------|-------------|
| `Insert<T,K>(item, clone)` | Add interval maintaining continuity |
| `Insert<T,K>(item, isEqual, clone)` | Add with custom equality for merging |
| `Update<T,K>(modify, clone, start, end)` | Update values in range with auto-split |
| `Find<T,K>(key)` | Find interval containing point |
| `Find<T,K>(start, end)` | Find intervals overlapping range |
| `FindRequired<T,K>(key)` | Find or throw if not found |
| `TrimStart<T,K>(newStart)` | Remove data before new start |
| `TrimEnd<T,K>(newEnd)` | Remove data after new end |
| `Rebuild<T,K>(isEqual)` | Consolidate adjacent intervals with equal values |
| `Merge<T,K>()` | Merge overlapping/adjacent intervals, preserving gaps |
| `Join<L,R,T,K>(right, convert)` | Combine two interval series |
| `Verify<T,K>(throwException)` | Validate continuity and non-overlap |
| `IsValid<T>()` | Check if interval start <= end |

## Built-in Types

| Type | Key | Next/Previous |
|------|-----|---------------|
| `DateInterval<V>` | `DateOnly` | +/- 1 day |
| `IntInterval<V>` | `int` | +/- 1 |
