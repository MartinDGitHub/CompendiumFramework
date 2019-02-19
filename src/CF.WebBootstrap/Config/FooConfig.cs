using CF.Common.Config;
using CF.WebBootstrap.Config.Options;
using Microsoft.Extensions.Options;

namespace CF.WebBootstrap.Config
{
    class FooConfig : IFooConfig
    {
        public string Foo { get; private set; }

        public FooConfig(IOptionsMonitor<RootOptions> options)
        {
            this.Foo = options.CurrentValue.FooOptions.Foo;
        }
    }
}
