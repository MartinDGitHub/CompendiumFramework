
namespace CF.Common.Authorization.Requirements
{
    public interface IClaimRequirement<T> : IRequirement
    {
        string Type { get; }

        T Value { get; }
    }
}
