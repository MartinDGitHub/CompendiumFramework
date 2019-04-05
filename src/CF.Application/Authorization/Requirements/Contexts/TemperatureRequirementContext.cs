using CF.Common.Authorization.Requirements;
using CF.Domain.Weather;

namespace CF.Application.Authorization.Requirements.Contexts
{
    public class TemperatureRequirementContext : IRequirementContext
    {
        public Temperature Temperature { get; }

        public TemperatureRequirementContext(Temperature temperature)
        {
            this.Temperature = temperature;
        }
    }
}
