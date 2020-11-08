using System;

namespace Titan.Core.Common
{
    internal interface IDateTime
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }
}
