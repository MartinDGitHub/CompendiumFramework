using CF.Common.Authorization.Requirements;
using CF.Domain.Weather;
using System;

namespace CF.Application.Authorization.Requirements
{
    public class TemperatureRangeRequirement : IRequirement
    {
        public TemperatureRange Range { get; }

        public TemperatureRangeRequirement(TemperatureRange range)
        {
            this.Range = range;
        }
    }
}
