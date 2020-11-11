using System;

namespace Titan.Core.Common
{
    internal class DateTimeWrapper : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
