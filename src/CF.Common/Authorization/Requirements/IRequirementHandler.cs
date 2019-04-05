using System.Threading.Tasks;

namespace CF.Common.Authorization.Requirements
{
    public interface IRequirementHandler<TRequirement>
        where TRequirement : IRequirement
    {
        Task<RequirementResult> HandleRequirementAsync(TRequirement requirement);
    }

    public interface IRequirementHandler<TRequirementContext, TRequirement> 
        where TRequirementContext : IRequirementContext 
        where TRequirement : IRequirement
    {
        Task<RequirementResult> HandleRequirementAsync(TRequirementContext context, TRequirement requirement);
    }
}
