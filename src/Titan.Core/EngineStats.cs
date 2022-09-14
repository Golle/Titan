using System.Collections.Concurrent;

namespace Titan.Core
{
    public enum SystemStats
    {
        PreUpdate,
        FixedUpdate,
        Update,
        PostUpdate
    }

    public class SystemStat
    {
        public double PreUpdate;
        public double FixedUpdate;
        public double PostUpdate;
        public double Update;
    }
    public static class EngineStats
    {
        private static readonly ConcurrentDictionary<string, double> _values = new ();
        private static readonly ConcurrentDictionary<string, SystemStat> _systemStats = new();

        public static int TotalLines => _values.Count + _systemStats.Count;
        public static void SetStats(string name, double value)
        {
            _values[name] = value;
        }

        public static void SetSystemStats(string name, SystemStats type, double value)
        {
            if (!_systemStats.TryGetValue(name, out var systemStats))
            {
                _systemStats[name] = systemStats = new SystemStat();
            }

            switch (type)
            {
                case SystemStats.PreUpdate:
                    systemStats.PreUpdate = value;
                    break;
                case SystemStats.Update:
                    systemStats.Update = value;
                    break;
                case SystemStats.PostUpdate:
                    systemStats.PostUpdate = value;
                    break;
                case SystemStats.FixedUpdate:
                    systemStats.FixedUpdate = value;
                    break;
            }
        }

        public static IDictionary<string, double> GetStats() => _values;
        public static IDictionary<string, SystemStat> GetSystemStats() => _systemStats;
    }
}
