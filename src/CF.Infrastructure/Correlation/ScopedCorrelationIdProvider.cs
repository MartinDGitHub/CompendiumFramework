using CF.Common.Correlation;
using shortid;
using shortid.Configuration;

namespace CF.Infrastructure.Correlation
{
    public class ScopedCorrelationIdProvider : IScopedCorrelationIdProvider
    {
        public string CorrelationId { get; } = ShortId.Generate(new GenerationOptions
        { 
            Length = 10,
            UseSpecialCharacters = false,
            UseNumbers = true,
        });
    }
}
