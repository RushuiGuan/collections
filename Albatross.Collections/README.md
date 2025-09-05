# Albatross.Collections

A comprehensive .NET library providing extension methods and data structures for enhanced collection operations. This library offers high-performance algorithms for searching, merging, batching, and manipulating collections with a focus on efficiency and ease of use.

## Features

### Collection Extensions
- **Batching**: Split collections into manageable chunks of specified sizes
- **Range Operations**: Add multiple items to collections efficiently
- **Functional Programming**: ForEach operations on enumerables
- **Dictionary Utilities**: GetOrAdd and TryGetAndRemove operations
- **Smart List Removal**: Multiple optimized algorithms for removing items from lists
  - `RemoveAny`: Automatic algorithm selection based on list size
  - `RemoveAny_FromRear`: Optimized rear-removal for better performance
  - `RemoveAny_WithNewList`: Memory-efficient removal for large lists
  - `TryGetOneAndRemove`: Find and remove single items

### Search Extensions
- **Binary Search**: High-performance binary search implementations
- **Value Lookups**: Find first value greater than, less than, or equal to targets
- **Sorted Collection Support**: Optimized searching for pre-sorted data

### Merge Operations
- **Full Outer Join**: Merge collections with comprehensive join logic
- **Flexible Handlers**: Support for matched, left join, and right join scenarios
- **Key-based Merging**: Join collections based on custom key selectors

## Prerequisites

- **.NET SDK**: 8.0 or later (for development and testing)
- **Runtime Compatibility**: .NET Standard 2.1 (compatible with .NET Core 2.1+, .NET 5+, .NET Framework 4.7.2+)
- **Dependencies**: No external runtime dependencies

## Installation

### Package Manager Console
```powershell
Install-Package Albatross.Collections
```

### .NET CLI
```bash
dotnet add package Albatross.Collections
```

### PackageReference
```xml
<PackageReference Include="Albatross.Collections" Version="7.5.14" />
```

### Development Setup
```bash
# Clone the repository
git clone https://github.com/RushuiGuan/collections.git
cd collections

# Restore dependencies
dotnet restore

# Build the project
dotnet build Albatross.Collections/Albatross.Collections.csproj

# Run tests
dotnet test Albatross.Collections.Test/Albatross.Collections.Test.csproj
```

## Example Usage

### Basic Collection Extensions

```csharp
using Albatross.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

// Batching items
var numbers = Enumerable.Range(1, 10);
foreach (var batch in numbers.Batch(3))
{
    Console.WriteLine($"Batch: {string.Join(", ", batch)}");
}
// Output: Batch: 1, 2, 3
//         Batch: 4, 5, 6
//         Batch: 7, 8, 9
//         Batch: 10

// Dictionary operations
var cache = new Dictionary<string, int>();
var value = cache.GetOrAdd("key1", () => ExpensiveCalculation());

// Remove items from list efficiently
var list = new List<int> { 1, 2, 3, 4, 5 };
var removed = list.RemoveAny(x => x % 2 == 0); // Remove even numbers
// list now contains: [1, 3, 5]
// removed contains: [2, 4]
```

### Binary Search Operations

```csharp
using Albatross.Collections;

// Search in sorted arrays
var sortedNumbers = new int[] { 1, 3, 5, 7, 9, 11 };

// Find first value >= target
var result = sortedNumbers.BinarySearchFirstValueGreaterOrEqual(6, x => x);
Console.WriteLine(result); // Output: 7

// Search with custom key selector
var people = new[]
{
    new { Name = "Alice", Age = 25 },
    new { Name = "Bob", Age = 30 },
    new { Name = "Charlie", Age = 35 }
};

var person = people.BinarySearchFirstLessOrEqual(28, x => x.Age);
Console.WriteLine(person?.Name); // Output: Alice
```

### Collection Merging

```csharp
using Albatross.Collections;

var oldItems = new[] { 
    new { Id = 1, Name = "Item1" }, 
    new { Id = 2, Name = "Item2" } 
};

var newItems = new[] { 
    new { Id = 2, Name = "Item2Updated" }, 
    new { Id = 3, Name = "Item3" } 
};

oldItems.Merge(newItems,
    src => src.Id,           // Source key selector
    dst => dst.Id,           // Destination key selector
    (src, dst) => {          // Handle matches (updates)
        Console.WriteLine($"Update: {dst.Name} -> {src.Name}");
    },
    src => {                 // Handle new items
        Console.WriteLine($"Add: {src.Name}");
    },
    dst => {                 // Handle removed items
        Console.WriteLine($"Remove: {dst.Name}");
    });
```

## Project Structure

```
collections/
├── Albatross.Collections/              # Main library
│   ├── Extensions.cs                   # Core collection extensions
│   ├── SearchExtensions.cs             # Binary search operations
│   ├── MergeExtensions.cs              # Collection merging utilities
│   └── Albatross.Collections.csproj    # Project file
├── Albatross.Collections.Test/         # Unit tests
│   ├── TestExtensions.cs               # Extension method tests
│   ├── TestSearch.cs                   # Search algorithm tests
│   ├── TestSortedData.cs               # Sorted data structure tests
│   └── Albatross.Collections.Test.csproj
├── Albatross.Collections.Intervals/    # Interval collections (separate package)
├── Benchmark.Collections/              # Performance benchmarking
├── collections.sln                     # Solution file
└── README.md                           # This file
```

## Running Unit Tests

The project includes comprehensive test coverage with 142+ unit tests.

### Run All Tests
```bash
dotnet test Albatross.Collections.Test/Albatross.Collections.Test.csproj
```

### Run Tests with Detailed Output
```bash
dotnet test Albatross.Collections.Test/Albatross.Collections.Test.csproj --verbosity normal
```

### Run Specific Test Class
```bash
dotnet test --filter "TestExtensions"
```

### Test Coverage
The test suite covers:
- All extension method variations and edge cases
- Binary search algorithm correctness
- Collection merging scenarios
- Sorted data structure operations
- Performance validation for different algorithms

## Contributing

We welcome contributions! Please follow these simple steps:

### Submitting Issues
1. Check existing issues to avoid duplicates
2. Provide clear reproduction steps
3. Include relevant code samples
4. Specify your .NET version and environment

### Pull Requests
1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/your-feature`
3. **Make** your changes with tests
4. **Ensure** all tests pass: `dotnet test`
5. **Commit** with clear messages: `git commit -m "Add feature X"`
6. **Push** to your fork: `git push origin feature/your-feature`
7. **Submit** a pull request with description of changes

### Development Guidelines
- Follow existing code style and conventions
- Add unit tests for new functionality
- Update documentation for public APIs
- Ensure backward compatibility when possible
- Run the full test suite before submitting

### Code Style
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and single-purpose
- Follow .NET naming conventions

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2019 Rushui Guan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```