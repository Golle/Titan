using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Titan.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SampleBenchmarks>();
        }
    }


    [MemoryDiagnoser]
    public class SampleBenchmarks
    {
        [Benchmark]
        public void GetLastName()
        {
            // do stuff
        }
    }
}
