using System.Threading.Tasks;

namespace CF.Common.Authorization.Policies
{
    /// <summary>
    /// Defines a standalone policy which authorizes based on its constituent requirements and how
    /// they are evaluated by their handlers.
    /// </summary>
    public interface IStandalonePolicy : IPolicy
    {
        Task<PolicyResult> AuthorizeAsync();
    }
}
