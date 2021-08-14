using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Titan.Core
{
    public static class EngineStats
    {

        private static ConcurrentDictionary<string, double> _values = new ();

        public static void SetStats(string name, double value)
        {
            _values[name] = value;
        }



        public static IDictionary<string, double> GetStats() => _values;
    }
}
