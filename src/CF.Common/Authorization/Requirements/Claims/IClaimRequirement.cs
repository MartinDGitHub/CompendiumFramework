namespace CF.Common.Authorization.Requirements.Claims
{
    public interface IClaimRequirement<T> : IRequirement
    {
        string Type { get; }

        T Value { get; }
    }
}
