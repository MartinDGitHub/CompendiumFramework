using CF.Common.Config;
using CF.WebBootstrap.Config.Options;
using Microsoft.Extensions.Options;

namespace CF.WebBootstrap.Config
{
    class RootConfig : IRootConfig
    {
        public RootConfig(IOptionsSnapshot<Root> options)
        {
        }
    }
}
