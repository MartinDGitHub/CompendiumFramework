using CF.Application.Authorization.Requirements.Contexts;
using CF.Common.Authorization.Policies;

namespace CF.Application.Authorization.Policies
{
    public interface IWarmTemperaturePolicy : IContextPolicy<TemperatureRequirementContext>
    {
    }
}
