# Release Notes

## 8.0.2

### Albatross.Collections

- Added `AddToGroup<K, T>` extension for `IDictionary<K, List<T>>`. It appends a value to the list stored under a key, creating the list when the key is absent, and returns the dictionary so calls can be chained.

  ```csharp
  var groups = new Dictionary<string, List<int>>();
  groups.AddToGroup("odd", 1).AddToGroup("even", 2).AddToGroup("odd", 3);
  // groups["odd"] == [1, 3], groups["even"] == [2]
  ```

### Albatross.Collections.Intervals

- No functional changes.

### Documentation

- Added XML documentation comments to the public API of both packages.
- Added the docfx documentation site and rewrote the package READMEs.

## 8.0.1 (2025-12-10)

### Albatross.Collections

- **Breaking:** removed `UnionAll<T>(this ICollection<T>, params IEnumerable<T>[])`. Use `AddRange` instead.
- Now targets `netstandard2.0` and `net8.0`. It previously targeted only `netstandard2.1`, so the package now also works on .NET Framework.
- Added `UnionIfNotNull<T>`, which appends the non-null items of `second` to `first`. There are overloads for reference types and for nullable value types.
- Added `AsEnumerable<T>(this T item)`, which wraps a single item in an `IEnumerable<T>`.

### Albatross.Collections.Intervals

- Added `Merge<T, K>`, which sorts the intervals by start and merges overlapping or adjacent ones into a single interval. It assumes every interval represents the same value, and it widens the first interval of each merged group in place instead of creating new instances.

## 8.0.0 (2025-09-05)

### Albatross.Collections

- **Breaking:** removed the sorted data types `ISortedData<T>`, `SortedArrayData<T, R>`, `SortedDataExtensions`, `IPosition`, `IntPosition`, `LongPosition` and `EndPosition`.
- **Breaking:** removed the obsolete `TryGetAndRemove<T>(this ICollection<T>, ...)`. Use `TryGetOneAndRemove` instead.
- **Breaking:** removed the obsolete `Replace<T, K>(this IList<T>, ...)`. Use `RemoveAny` and `AddRange` instead.
- Fixed the package `RepositoryUrl`, which pointed to the codegen repository.

### Albatross.Collections.Intervals

- No functional changes.
