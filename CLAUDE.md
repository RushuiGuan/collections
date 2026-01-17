# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Test Commands

```bash
# Build entire solution
dotnet build collections.sln

# Build specific project
dotnet build Albatross.Collections/Albatross.Collections.csproj

# Run all tests
dotnet test collections.sln

# Run tests for a specific project
dotnet test Albatross.Collections.Test/Albatross.Collections.Test.csproj
dotnet test Albatross.Collections.Intervals.Test/Albatross.Collections.Intervals.Test.csproj

# Run a specific test class
dotnet test --filter "TestExtensions"

# Run benchmarks
dotnet run -c Release --project Benchmark.Collections/Benchmark.Collections.csproj

# Generate documentation (requires docfx)
docfx docfx_project/docfx.json
```

## Architecture Overview

This is a .NET collection utilities library with two main packages:

### Albatross.Collections
Core collection extension methods targeting both netstandard2.0 and net8.0:
- `Extensions.cs` - Batching, AddRange, ForEach, dictionary GetOrAdd/TryGetAndRemove, list item removal with automatic algorithm selection (RemoveAny switches between FromRear and WithNewList based on ListItemRemovalAlgoCutoff=100)
- `SearchExtensions.cs` - Binary search implementations for sorted lists (BinarySearchFirstValueGreaterOrEqual, BinarySearchFirstLessOrEqual) with separate methods for struct vs class types
- `MergeExtensions.cs` - Full outer join between collections with handlers for matched/notMatchedByDst/notMatchedBySrc scenarios, including async variant

### Albatross.Collections.Intervals
Specialized library for continuous, non-overlapping interval management (net8.0 only):
- `IClosedInterval<K>` / `IClosedInterval<K,V>` - Core interfaces requiring `Next()` and `Previous()` static abstract methods for interval arithmetic
- `DateInterval<V>` and `IntInterval<V>` - Built-in implementations for DateOnly and int keys
- `ClosedIntervalExtensions.cs` - Operations maintaining continuity:
  - `Insert` - Adds interval while auto-merging adjacent intervals with equal values and preventing gaps
  - `Update` - Modifies values in range, auto-splitting intervals as needed
  - `Find` - Point and range queries
  - `Join` - Combines two interval series
  - `Rebuild` - Consolidates adjacent intervals with matching values
  - `Verify` - Validates continuity and non-overlap

The intervals library enforces that series remain continuous (no gaps) and non-overlapping through all operations.

## Test Projects

Tests use xUnit with FluentAssertions. Test projects target net10.0 and include stress tests for interval operations (e.g., `CloseIntervalUpdateStressTester`, `InsertClosedIntervals_StressTest`).
