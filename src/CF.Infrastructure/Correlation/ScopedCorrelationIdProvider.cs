using CF.Common.Correlation;
using shortid;

namespace CF.Infrastructure.Correlation
{
    public class ScopedCorrelationIdProvider : IScopedCorrelationIdProvider
    {
        public string CorrelationId { get; } = ShortId.Generate(useNumbers: true, useSpecial: false, length: 10);

    }
}
