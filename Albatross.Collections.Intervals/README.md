# Albatross.Collections.Intervals

A comprehensive .NET library for managing continuous and non-overlapping intervals. This library provides efficient data structures and algorithms for working with interval-based data, where keys are ranges instead of single values. It's particularly useful for time-series data, date ranges, and any scenario where you need to efficiently store and query data that spans intervals.

## Features

### Core Interval Types
- **Closed Intervals**: Intervals with inclusive start and end points (`IClosedInterval<K>`, `IClosedInterval<K,V>`)
- **Open Intervals**: Intervals with exclusive start and end points (`IOpenInterval<T>`)
- **Mixed Intervals**: Open-closed interval combinations for flexible boundary handling

### Built-in Implementations
- **IntInterval**: Integer-based intervals with value support (`IntInterval<V>`)
- **DateInterval**: Date-based intervals using `DateOnly` (`DateInterval<V>`)
- **Custom Types**: Extensible interface for any comparable type

### Advanced Operations
- **Insert**: Add new intervals while maintaining continuity and non-overlap
- **Update**: Modify values within specified ranges with automatic splitting
- **Find**: Efficient lookup of intervals containing specific values or ranges
- **Join**: Combine multiple interval series with flexible join logic
- **Trim**: Adjust interval boundaries (start/end trimming)
- **Rebuild**: Consolidate adjacent intervals with identical values
- **Verify**: Validate interval continuity and detect overlaps

### Key Benefits
- **Continuous Series**: Automatic maintenance of gap-free interval sequences
- **Non-overlapping**: Built-in prevention of interval conflicts
- **Efficient Storage**: Optimal for data with repeated values across ranges
- **Time-series Support**: Perfect for tracking changes over time
- **Type Safety**: Strongly-typed generic interfaces for compile-time validation

## Prerequisites

- **.NET SDK**: 8.0 or later (for development and testing)
- **Runtime Compatibility**: .NET 8.0+
- **Dependencies**: No external runtime dependencies

## Installation

### Package Manager Console
```powershell
Install-Package Albatross.Collections.Intervals
```

### .NET CLI
```bash
dotnet add package Albatross.Collections.Intervals
```

### PackageReference
```xml
<PackageReference Include="Albatross.Collections.Intervals" Version="7.5.14" />
```

### Development Setup
```bash
# Clone the repository
git clone https://github.com/RushuiGuan/collections.git
cd collections

# Restore dependencies
dotnet restore

# Build the project
dotnet build Albatross.Collections.Intervals/Albatross.Collections.Intervals.csproj

# Run tests
dotnet test Albatross.Collections.Intervals.Test/Albatross.Collections.Intervals.Test.csproj
```

## Example Usage

### Basic Date Intervals

```csharp
using Albatross.Collections.Intervals;

// Create date intervals with values
var intervals = new List<DateInterval<string>>();

// Insert intervals - automatically maintains continuity
intervals = intervals.Insert(
    new DateInterval<string>(new DateOnly(2023, 1, 1), new DateOnly(2023, 1, 31), "January Data"), 
    x => x
).ToList();

intervals = intervals.Insert(
    new DateInterval<string>(new DateOnly(2023, 2, 1), new DateOnly(2023, 2, 28), "February Data"), 
    x => x
).ToList();

// Find intervals containing a specific date
var result = intervals.Find(new DateOnly(2023, 1, 15));
Console.WriteLine(result?.Value); // Output: "January Data"

// Find all intervals overlapping a range
var rangeResults = intervals.Find(
    new DateOnly(2023, 1, 20), 
    new DateOnly(2023, 2, 10)
);
```

### Integer Intervals with Updates

```csharp
using Albatross.Collections.Intervals;

// Create integer intervals
var intervals = new List<IntInterval<decimal>>();

// Insert base interval
intervals = intervals.Insert(
    new IntInterval<decimal>(1, 100, 10.5m), 
    x => x
).ToList();

// Update values in a specific range (automatically splits intervals)
intervals.Update(
    interval => interval.Value = 25.0m,  // Modification action
    1, 50,                               // Range to update
    x => x                               // Clone function
);

// Verify continuity and non-overlap
bool isValid = intervals.Verify(throwException: false);
```

### Advanced Operations

```csharp
using Albatross.Collections.Intervals;

var intervals = new List<DateInterval<int>>();

// Create sample data
intervals = intervals.Insert(new DateInterval<int>(DateOnly.Parse("2023-01-01"), DateOnly.Parse("2023-01-31"), 100), x => x).ToList();
intervals = intervals.Insert(new DateInterval<int>(DateOnly.Parse("2023-02-01"), DateOnly.Parse("2023-02-28"), 100), x => x).ToList();
intervals = intervals.Insert(new DateInterval<int>(DateOnly.Parse("2023-03-01"), DateOnly.Parse("2023-03-31"), 200), x => x).ToList();

// Rebuild to consolidate adjacent intervals with same values
var consolidated = intervals.Rebuild((a, b) => a.Value == b.Value).ToList();

// Trim operations
var trimmed = intervals.TrimStart(DateOnly.Parse("2023-01-15")).ToList();
var endTrimmed = intervals.TrimEnd(DateOnly.Parse("2023-02-15")).ToList();

// Join two interval series
var series1 = new List<DateInterval<string>> { /* ... */ };
var series2 = new List<DateInterval<int>> { /* ... */ };

var joined = series1.Join<DateInterval<string>, DateInterval<int>, DateInterval<string>, DateOnly>(
    series2, 
    (left, right) => new DateInterval<string>(left.StartInclusive, left.EndInclusive, $"{left.Value}+{right.Value}")
);
```

## Project Structure

```
Albatross.Collections.Intervals/
├── Albatross.Collections.Intervals.csproj  # Project file
├── README.md                               # This file
├── ClosedInterval.cs                       # Core interval interfaces
├── ClosedIntervalExtensions.cs             # Extension methods for closed intervals
├── IntInterval.cs                          # Integer interval implementation
├── DateInterval.cs                         # Date interval implementation
├── OpenInterval.cs                         # Open interval interfaces
├── OpenClosedInterval.cs                   # Mixed interval types
├── IntervalException.cs                    # Custom exceptions
├── ComparableExtensions.cs                 # Utility extensions
└── ICloneable.cs                          # Cloning interface

Albatross.Collections.Intervals.Test/       # Unit tests
├── TestInsertClosedIntervals.cs            # Insert operation tests
├── TestUpdateCloseIntervals.cs             # Update operation tests
├── TestClosedIntervalExtentions.cs         # Extension method tests
├── CloseIntervalUpdateStressTester.cs      # Stress testing utilities
├── InsertClosedIntervals_StressTest.cs     # Insert stress tests
├── DateLevelTestRunner.cs                  # Date-specific test utilities
└── [Additional test files...]              # Comprehensive test coverage
```

## Running Unit Tests

The project includes comprehensive test coverage with 86+ unit tests covering all major functionality.

### Run All Tests
```bash
dotnet test Albatross.Collections.Intervals.Test/Albatross.Collections.Intervals.Test.csproj
```

### Run Tests with Detailed Output
```bash
dotnet test Albatross.Collections.Intervals.Test/Albatross.Collections.Intervals.Test.csproj --verbosity normal
```

### Run Specific Test Class
```bash
dotnet test --filter "TestInsertClosedIntervals"
```

### Test Coverage
The test suite covers:
- Interval insertion with continuity validation
- Update operations with automatic splitting
- Find operations for point and range queries
- Join operations between different interval series
- Trim operations for boundary adjustments
- Rebuild operations for consolidation
- Edge cases and error conditions
- Stress testing with large datasets

## Contributing

We welcome contributions! Please follow these guidelines:

### Getting Started
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Add or update tests as needed
5. Ensure all tests pass (`dotnet test`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Code Style
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and single-purpose
- Follow .NET naming conventions
- Maintain backward compatibility when possible

### Testing Requirements
- Add unit tests for new functionality
- Ensure existing tests continue to pass
- Include edge case testing
- Use FluentAssertions for test assertions
- Follow existing test patterns and naming

### Pull Request Process
- Provide clear description of changes
- Reference any related issues
- Ensure CI/CD checks pass
- Request review from maintainers

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

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
