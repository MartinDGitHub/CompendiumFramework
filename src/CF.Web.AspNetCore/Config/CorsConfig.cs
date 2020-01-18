using CF.Web.AspNetCore.Config.Sections;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CF.Web.AspNetCore.Config
{
    // Not registered for DI injection outside of this assembly.
    internal class CorsConfig
    {
        public Dictionary<string, string[]> OriginsByPolicy { get; private set; }

        public CorsConfig(IOptionsMonitor<Root> options)
        {
            var corsOptions = options.CurrentValue?.Cors ?? throw new Exception($"Options for section [{nameof(Sections.Cors)}] were not loaded.");

            this.OriginsByPolicy = corsOptions.OriginsByPolicy;
        }
    }
}
