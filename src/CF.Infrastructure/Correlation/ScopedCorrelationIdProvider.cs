using CF.Common.Correlation;
using shortid;

namespace CF.Infrastructure.Correlation
{
    internal class ScopedCorrelationIdProvider : IScopedCorrelationIdProvider
    {
        public string CorrelationId { get; } = ShortId.Generate(useNumbers: true, useSpecial: false, length: 10);

    }
}
