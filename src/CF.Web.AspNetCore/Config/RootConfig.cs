using CF.Common.Config;
using CF.Web.AspNetCore.Config.Sections;
using Microsoft.Extensions.Options;

namespace CF.Web.AspNetCore.Config
{
    internal class RootConfig : IRootConfig
    {
        public RootConfig(IOptionsMonitor<Root> options)
        {
        }
    }
}
