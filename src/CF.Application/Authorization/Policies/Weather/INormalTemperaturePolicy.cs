using CF.Application.Authorization.Requirements.Contexts;
using CF.Common.Authorization.Policies;

namespace CF.Application.Authorization.Policies.Weather
{
    public interface INormalTemperaturePolicy : IContextPolicy<TemperatureRequirementContext>
    {
    }
}
