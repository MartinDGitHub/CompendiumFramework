using CF.Common.Authorization.Requirements;
using System.Collections.Generic;

namespace CF.Common.Authorization.Policies
{
    /// <summary>
    /// A policy that has associated requirements.
    /// </summary>
    /// <typeparam name="TRequirement">The type of requirement.</typeparam>
    public interface IPolicy<TRequirement> : IPolicy where TRequirement: IRequirement
    {
        IEnumerable<TRequirement> Requirements { get; }
    }

    /// <summary>
    /// A common marker interface for all authorization policies.
    /// </summary>
    public interface IPolicy
    {
    }
}
