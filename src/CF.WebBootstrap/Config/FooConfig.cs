using CF.Common.Config;
using Microsoft.Extensions.Options;

namespace CF.WebBootstrap.DI
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
