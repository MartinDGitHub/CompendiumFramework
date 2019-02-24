using CF.Common.Config;
using CF.Infrastructure.Config.Options;
using Microsoft.Extensions.Options;
using System;

namespace CF.Infrastructure.Config
{
    class FooConfig : IFooConfig
    {
        public string Foo { get; private set; }

        public FooConfig(IOptionsSnapshot<RootOptions> options)
        {
            this.Foo = options.Value?.FooOptions?.Foo ?? throw new Exception($"Options for [{nameof(FooOptions)}] were not loaded.");
        }
    }
}
