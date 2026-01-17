# Albatross.Collections

A .NET library providing extension methods for collections including batching, searching, merging, and efficient list manipulation.

## Installation

```bash
dotnet add package Albatross.Collections
```

## Quick Start

### Batching
```csharp
using Albatross.Collections;

var numbers = Enumerable.Range(1, 10);
foreach (var batch in numbers.Batch(3)) {
    Console.WriteLine(string.Join(", ", batch));
}
// Output: 1, 2, 3
//         4, 5, 6
//         7, 8, 9
//         10
```

### Dictionary Operations
```csharp
var cache = new Dictionary<string, int>();
var value = cache.GetOrAdd("key", () => ComputeExpensiveValue());

if (cache.TryGetAndRemove("key", out var removed)) {
    Console.WriteLine($"Removed: {removed}");
}
```

### List Item Removal
```csharp
var list = new List<int> { 1, 2, 3, 4, 5 };
var removed = list.RemoveAny(x => x % 2 == 0);
// list: [1, 3, 5], removed: [2, 4]
```

### Binary Search (sorted data)
```csharp
var sorted = new int[] { 1, 3, 5, 7, 9, 11 };

var result = sorted.BinarySearchFirstValueGreaterOrEqual(6, x => x);
Console.WriteLine(result); // 7

var result2 = sorted.BinarySearchFirstValueLessOrEqual(6, x => x);
Console.WriteLine(result2); // 5
```

### Collection Merging (Full Outer Join)
```csharp
var existing = new[] { new { Id = 1, Name = "A" }, new { Id = 2, Name = "B" } };
var incoming = new[] { new { Id = 2, Name = "B2" }, new { Id = 3, Name = "C" } };

existing.Merge(incoming,
    src => src.Id,
    dst => dst.Id,
    (src, dst) => Console.WriteLine($"Update: {dst.Name} -> {src.Name}"),
    src => Console.WriteLine($"Add: {src.Name}"),
    dst => Console.WriteLine($"Remove: {dst.Name}"));
// Output: Update: B -> B2
//         Remove: A
//         Add: C
```

## API Reference

| Method | Description |
|--------|-------------|
| `Batch<T>(size)` | Split enumerable into arrays of specified size |
| `GetOrAdd<K,T>(key, factory)` | Get or create dictionary value |
| `TryGetAndRemove<K,T>(key, out value)` | Get and remove dictionary value |
| `RemoveAny<T>(predicate)` | Remove matching items, returns removed items |
| `TryGetOneAndRemove<T>(predicate, out item)` | Find and remove single item |
| `BinarySearchFirstValueGreaterOrEqual<T,K>(key, selector)` | Find first value >= key |
| `BinarySearchFirstValueLessOrEqual<T,K>(key, selector)` | Find first value <= key |
| `Merge<Src,Dst,TKey>(...)` | Full outer join with match/add/remove handlers |
| `MergeAsync<Src,Dst,TKey>(...)` | Async version of Merge |
| `AddRange<T>(items)` | Add multiple items to ICollection |
| `ForEach<T>(action)` | Execute action on each item |

## License

MIT License - see [LICENSE](../LICENSE)
