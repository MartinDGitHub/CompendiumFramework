using CF.Common.Config;
using CF.Web.AspNetCore.Config.Sections;
using Microsoft.Extensions.Options;
using System;

namespace CF.Web.AspNetCore.Config
{
    internal class EnvironmentConfig : IEnvironmentConfig
    {
        public string Name { get; private set; }

        public EnvironmentConfig(IOptionsMonitor<Root> options)
        {
            var environmentOptions = options.CurrentValue?.Environment ?? throw new Exception($"Options for section [{nameof(Sections.Environment)}] were not loaded.");

            if (string.IsNullOrWhiteSpace(environmentOptions.Name))
            {
                throw new Exception($"The [{nameof(Sections.Environment.Name)}] property of section [{nameof(Sections.Environment)}] was null or whitespace.");
            }
            this.Name = environmentOptions.Name;
        }
    }
}
