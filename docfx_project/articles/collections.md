# Albatross.Collections

A comprehensive .NET library providing extension methods and utilities for enhanced collection operations.

## Installation

```bash
dotnet add package Albatross.Collections
```

## Batching

Split collections into smaller chunks of a specified size.

```csharp
using Albatross.Collections;

var numbers = Enumerable.Range(1, 10);
foreach (var batch in numbers.Batch(3)) {
    Console.WriteLine(string.Join(", ", batch));
}
// Output:
// 1, 2, 3
// 4, 5, 6
// 7, 8, 9
// 10
```

The last batch contains the remaining items if the total count isn't evenly divisible by the batch size.

## Dictionary Extensions

### GetOrAdd

Retrieve a value from a dictionary, or add it if the key doesn't exist.

```csharp
var cache = new Dictionary<string, ExpensiveObject>();

// Value is computed only if key doesn't exist
var value = cache.GetOrAdd("key1", () => new ExpensiveObject());

// Subsequent calls return cached value
var same = cache.GetOrAdd("key1", () => new ExpensiveObject());
```

### TryGetAndRemove

Atomically retrieve and remove a value from a dictionary.

```csharp
var dict = new Dictionary<string, int> { ["key"] = 42 };

if (dict.TryGetAndRemove("key", out var value)) {
    Console.WriteLine($"Removed: {value}"); // 42
}
// dict no longer contains "key"
```

### Where

Conditional dictionary lookup with optional predicate filtering.

```csharp
var users = new Dictionary<int, User> {
    [1] = new User { Name = "Alice", Active = true },
    [2] = new User { Name = "Bob", Active = false }
};

// Get user only if active
var activeUser = users.Where(1, u => u.Active); // Returns Alice
var inactiveUser = users.Where(2, u => u.Active); // Returns null
```

## Collection Extensions

### AddRange

Add multiple items to any `ICollection<T>`.

```csharp
ICollection<int> collection = new HashSet<int>();
collection.AddRange(new[] { 1, 2, 3, 4, 5 });
```

### AddIfNotNull

Add items only if they're not null.

```csharp
var list = new List<string>();
string? maybeNull = GetOptionalValue();
list.AddIfNotNull(maybeNull, "definiteValue", anotherMaybeNull);
```

### ForEach

Execute an action on each item in an enumerable.

```csharp
var items = new[] { 1, 2, 3 };
items.ForEach(x => Console.WriteLine(x));
```

### AsEnumerable

Wrap a single item as an enumerable.

```csharp
var single = "item".AsEnumerable();
// Returns IEnumerable<string> with one element
```

### UnionIfNotNull

Concatenate enumerables, filtering out null values.

```csharp
var list = new[] { "a", "b" };
string? nullable = null;
var result = list.UnionIfNotNull(nullable, "c"); // ["a", "b", "c"]
```

## List Item Removal

Multiple optimized algorithms for removing items from lists.

### RemoveAny

Automatically selects the best algorithm based on list size. Uses `RemoveAny_FromRear` for lists ≤100 items, `RemoveAny_WithNewList` for larger lists.

```csharp
var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
var removed = list.RemoveAny(x => x % 2 == 0);

// list: [1, 3, 5, 7, 9]
// removed: [2, 4, 6, 8, 10]
```

### RemoveAny_FromRear

Iterates from the end of the list, removing matching items. O(n) complexity, efficient for smaller lists.

```csharp
var removed = list.RemoveAny_FromRear(x => x > 5);
```

### RemoveAny_WithNewList

Creates a new list with non-matching items, then replaces the original. Better for large lists where many items are removed.

```csharp
var removed = list.RemoveAny_WithNewList(x => x > 5);
```

### TryGetOneAndRemove

Find and remove a single matching item.

```csharp
var list = new List<User> { new User("Alice"), new User("Bob") };

if (list.TryGetOneAndRemove(u => u.Name == "Alice", out var alice)) {
    Console.WriteLine($"Found and removed: {alice.Name}");
}
```

### Algorithm Cutoff

The default cutoff is 100 items. You can customize it:

```csharp
// Force using the new list algorithm for lists > 50 items
var removed = list.RemoveAny(x => x > 5, algoCutoff: 50);
```

## Binary Search

High-performance search operations for sorted collections.

### BinarySearchFirstValueGreaterOrEqual

Find the first value greater than or equal to a target.

```csharp
var sorted = new int[] { 1, 3, 5, 7, 9, 11 };

var result = sorted.BinarySearchFirstValueGreaterOrEqual(6, x => x);
Console.WriteLine(result); // 7

var result2 = sorted.BinarySearchFirstValueGreaterOrEqual(5, x => x);
Console.WriteLine(result2); // 5

var result3 = sorted.BinarySearchFirstValueGreaterOrEqual(12, x => x);
Console.WriteLine(result3); // null (default)
```

### BinarySearchFirstValueLessOrEqual

Find the first value less than or equal to a target.

```csharp
var sorted = new int[] { 1, 3, 5, 7, 9, 11 };

var result = sorted.BinarySearchFirstValueLessOrEqual(6, x => x);
Console.WriteLine(result); // 5

var result2 = sorted.BinarySearchFirstValueLessOrEqual(0, x => x);
Console.WriteLine(result2); // null (default)
```

### Custom Key Selectors

Search complex objects using a key selector function.

```csharp
var people = new Person[] {
    new("Alice", 25),
    new("Bob", 30),
    new("Charlie", 35),
    new("Diana", 40)
};

// Find person with age <= 32
var person = people.BinarySearchFirstLessOrEqual(32, p => p.Age);
Console.WriteLine(person?.Name); // Bob

// Find person with age >= 32
var person2 = people.BinarySearchFirstGreaterOrEqual(32, p => p.Age);
Console.WriteLine(person2?.Name); // Charlie
```

### Struct vs Class Methods

The library provides separate methods for value types (structs) and reference types (classes):

- `BinarySearchFirstValueGreaterOrEqual` / `BinarySearchFirstValueLessOrEqual` - for structs
- `BinarySearchFirstGreaterOrEqual` / `BinarySearchFirstLessOrEqual` - for classes

## Collection Merging

Perform full outer joins between collections with flexible handling of matches, additions, and removals.

### Basic Merge

```csharp
var existing = new[] {
    new Product(1, "Widget", 10.00m),
    new Product(2, "Gadget", 20.00m),
    new Product(3, "Gizmo", 30.00m)
};

var incoming = new[] {
    new Product(2, "Gadget Pro", 25.00m),  // Updated
    new Product(4, "Doohickey", 15.00m)    // New
};

existing.Merge(
    incoming,
    src => src.Id,                          // Source key selector
    dst => dst.Id,                          // Destination key selector
    (src, dst) => {                         // Matched - update
        Console.WriteLine($"Update: {dst.Name} -> {src.Name}");
    },
    src => {                                // Not in destination - add
        Console.WriteLine($"Add: {src.Name}");
    },
    dst => {                                // Not in source - remove
        Console.WriteLine($"Remove: {dst.Name}");
    }
);

// Output:
// Update: Gadget -> Gadget Pro
// Remove: Widget
// Remove: Gizmo
// Add: Doohickey
```

### Async Merge

For operations that require async processing:

```csharp
await existing.MergeAsync(
    incoming,
    src => src.Id,
    dst => dst.Id,
    async (src, dst) => await UpdateInDatabaseAsync(src, dst),
    async src => await InsertIntoDatabaseAsync(src),
    async dst => await DeleteFromDatabaseAsync(dst)
);
```

### Handling Default Keys

Items with default key values (e.g., `0` for int, `null` for reference types) are treated as new items and passed to the `notMatchedByDst` handler.

```csharp
var incoming = new[] {
    new Product(0, "New Item", 5.00m),  // Id = 0 (default)
};

existing.Merge(
    incoming,
    src => src.Id,
    dst => dst.Id,
    matched: null,
    notMatchedByDst: src => Console.WriteLine($"New: {src.Name}"),  // Called for Id=0
    notMatchedBySrc: null
);
```

## API Reference

| Method | Description |
|--------|-------------|
| `Batch<T>(size)` | Split enumerable into arrays of specified size |
| `AddRange<T>(items)` | Add multiple items to ICollection |
| `AddIfNotNull<T>(items)` | Add non-null items to collection |
| `ForEach<T>(action)` | Execute action on each item |
| `AsEnumerable<T>()` | Wrap single item as enumerable |
| `UnionIfNotNull<T>(items)` | Concatenate filtering nulls |
| `GetOrAdd<K,T>(key, factory)` | Get or create dictionary value |
| `TryGetAndRemove<K,T>(key, out value)` | Get and remove dictionary value |
| `Where<K,T>(key, predicate)` | Conditional dictionary lookup |
| `WhereValue<K,T>(key, predicate)` | Conditional lookup for value types |
| `RemoveAny<T>(predicate, cutoff)` | Remove matching items with auto algorithm selection |
| `RemoveAny_FromRear<T>(predicate)` | Remove from rear, good for small lists |
| `RemoveAny_WithNewList<T>(predicate)` | Remove via new list, good for large lists |
| `TryGetOneAndRemove<T>(predicate, out item)` | Find and remove single item |
| `BinarySearchFirstValueGreaterOrEqual<T,K>(key, selector)` | Find first struct >= key |
| `BinarySearchFirstValueLessOrEqual<T,K>(key, selector)` | Find first struct <= key |
| `BinarySearchFirstGreaterOrEqual<T,K>(key, selector)` | Find first class >= key |
| `BinarySearchFirstLessOrEqual<T,K>(key, selector)` | Find first class <= key |
| `Merge<Src,Dst,TKey>(...)` | Full outer join with handlers |
| `MergeAsync<Src,Dst,TKey>(...)` | Async full outer join |
