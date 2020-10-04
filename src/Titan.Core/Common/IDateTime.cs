using System;

namespace Titan.Core.Logging
{
    internal interface IDateTime
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }
}
