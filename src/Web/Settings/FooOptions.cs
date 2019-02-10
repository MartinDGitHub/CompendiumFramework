using CF.Core.Config;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Settings
{
    class FooOptions
    {
        public string Foo { get; set; }

        public BarOptions BarOptions { get; set; }
    }
}
