using CF.Common.Authorization.Requirements.Contexts;
using System;

namespace CF.Application.Authorization.Requirements.Contexts
{
    public class TemperatureRequirementContext : IRequirementContext
    {
        public int Temperature { get; }

        public TemperatureRequirementContext(int temperature) => (this.Temperature) = temperature;
    }
}
