using CF.Common.Config;
using CF.Web.AspNetCore.Config.Sections;
using Microsoft.Extensions.Options;
using System;

namespace CF.Web.AspNetCore.Config
{
    internal class DomainConfig : IDomainConfig
    {
        public string Name { get; private set; }

        public DomainConfig(IOptionsMonitor<Root> options)
        {
            var domainOptions = options.CurrentValue?.Domain ?? throw new Exception($"Options for section [{nameof(Sections.Domain)}] were not loaded.");

            if (string.IsNullOrWhiteSpace(domainOptions.Name))
            {
                throw new Exception($"The [{nameof(Sections.Domain.Name)}] property of section [{nameof(Sections.Domain)}] was null or whitespace.");
            }
            this.Name = domainOptions.Name;
        }
    }
}
