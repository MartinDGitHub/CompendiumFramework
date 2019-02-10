using CF.Core.Config;
using Microsoft.Extensions.Options;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using Web.Settings;

namespace CF.Core.DI
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
