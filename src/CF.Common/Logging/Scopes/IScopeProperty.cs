namespace CF.Common.Logging
{
    public interface IScopeProperty<TValue>
    {
        string Name { get; }
        TValue Value { get; }
    }
}
