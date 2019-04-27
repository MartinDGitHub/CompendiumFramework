using CF.Common.Config;
using CF.Web.AspNetCore.Config.Options;
using Microsoft.Extensions.Options;

namespace CF.Web.AspNetCore.Config
{
    class RootConfig : IRootConfig
    {
        public RootConfig(IOptionsSnapshot<Root> options)
        {
        }
    }
}
