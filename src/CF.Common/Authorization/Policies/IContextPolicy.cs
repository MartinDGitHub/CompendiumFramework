using System.Threading.Tasks;

namespace CF.Common.Authorization.Policies
{
    /// <summary>
    /// Defines a context policy which authorizes based on its constituent requirements and how
    /// they are evaluated by their handlers using the context specified by the policy.
    /// </summary>
    /// <typeparam name="TRequirementContext">The requirement context type.</typeparam>
    public interface IContextPolicy<in TRequirementContext> : IPolicy where TRequirementContext : class
    {
        Task<PolicyResult> AuthorizeAsync(TRequirementContext context);
    }
}
