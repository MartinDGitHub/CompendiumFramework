using CF.Common.Authorization.Requirements.Contexts;
using System.Threading.Tasks;

namespace CF.Common.Authorization.Requirements.Handlers
{
    public interface IRequirementHandler<TRequirement>
        where TRequirement : IRequirement
    {
        Task<bool> HandleRequirementAsync(TRequirement requirement);
    }

    public interface IRequirementHandler<TRequirementContext, TRequirement> 
        where TRequirementContext : IRequirementContext 
        where TRequirement : IRequirement
    {
        Task<bool> HandleRequirementAsync(TRequirementContext context, TRequirement requirement);
    }
}
