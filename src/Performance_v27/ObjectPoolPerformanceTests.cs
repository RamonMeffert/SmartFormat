﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SmartFormat.Core.Settings;
using SmartFormat.Extensions;

namespace SmartFormat.Performance
{
    /*
// * Summary *

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.1348 (21H1/May2021Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=5.0.403
  [Host]   : .NET 5.0.12 (5.0.1221.52207), X64 RyuJIT
  .NET 5.0 : .NET 5.0.12 (5.0.1221.52207), X64 RyuJIT

Job=.NET 5.0  Runtime=.NET 5.0

|         Method |     N |     Mean |   Error |  StdDev |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------- |------ |---------:|--------:|--------:|-----------:|------:|------:|----------:|
| ObjectPoolTest | 10000 | 223.9 ms | 1.48 ms | 1.38 ms | 21333.3333 |     - |     - |    172 MB |

// * Hints *
Outliers
  ObjectPoolPerformanceTests.ObjectPoolTest: .NET 5.0 -> 1 outlier  was  detected (221.12 ms)

// * Legends *
  N         : Value of the 'N' parameter
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Ratio     : Mean of the ratio distribution ([Current]/[Baseline])
  Gen 0     : GC Generation 0 collects per 1000 operations
  Gen 1     : GC Generation 1 collects per 1000 operations
  Gen 2     : GC Generation 2 collects per 1000 operations
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ms      : 1 Millisecond (0.001 sec)
    */


    [SimpleJob(RuntimeMoniker.Net50)]
    [MemoryDiagnoser]
    // [RPlotExporter]
    public class ObjectPoolPerformanceTests
    {
        private readonly SmartFormatter _formatter;
        private readonly List<int> _list = new() { 1, 2, 3 };
        
        public ObjectPoolPerformanceTests()
        {
            _formatter = new SmartFormatter();
            var listSourceAndFormat = new ListFormatter(_formatter);
            _formatter.AddExtensions(listSourceAndFormat, new ReflectionSource(_formatter), new DefaultSource(_formatter));
            _formatter.AddExtensions(listSourceAndFormat, new DefaultFormatter());
        }

        [Params(10000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark(Baseline = false)]
        public void ObjectPoolTest()
        {
            const string indexPlaceholders = "All items: {0.0}, {0.1}, and {0.2}";
            const string listPlaceholders = "Total items: {0.Count}. All items: {0:list:{}|, |, and }";

            for (var i = 0; i < N; i++)
            {
                _ = _formatter.Format(indexPlaceholders, _list);
                _ = _formatter.Format(listPlaceholders, _list);
            }
        }
    }
}
