using CF.Common.Config;
using CF.Web.AspNetCore.Config.Options;
using Microsoft.Extensions.Options;
using System;

namespace CF.Web.AspNetCore.Config
{
    class EnvironmentConfig : IEnvironmentConfig
    {
        public string Name { get; private set; }

        public EnvironmentConfig(IOptionsMonitor<Root> options)
        {
            var environmentOptions = options.CurrentValue?.Environment ?? throw new Exception($"Options for [{nameof(Options.Environment)}] were not loaded.");

            if (string.IsNullOrWhiteSpace(environmentOptions.Name))
            {
                throw new Exception($"The [{nameof(Options.Environment.Name)}] property of [{nameof(Options.Environment)}] options was null or whitespace.");
            }
            this.Name = environmentOptions.Name;
        }
    }
}
