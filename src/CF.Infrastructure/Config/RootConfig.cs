using CF.Common.Config;
using CF.Infrastructure.Config.Options;
using Microsoft.Extensions.Options;

namespace CF.Infrastructure.Config
{
    class RootConfig : IRootConfig
    {
        public RootConfig(IOptionsSnapshot<RootOptions> options)
        {
        }
    }
}
