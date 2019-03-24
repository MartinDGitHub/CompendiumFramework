using CF.Application.Authorization.Requirements;
using CF.Application.Authorization.Requirements.Contexts;
using CF.Common.Authorization.Policies;
using CF.Common.Authorization.Requirements.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Application.Authorization.Policies
{
    internal class WarmTemperaturePolicy : IWarmTemperaturePolicy
    {
        private readonly IRequirementHandler<TemperatureRequirementContext, TemperatureRangeRequirement> _handler;

        private readonly IEnumerable<TemperatureRangeRequirement> _temperatureRangeRequirements = new[] { new TemperatureRangeRequirement(5, 30) };

        public WarmTemperaturePolicy(IRequirementHandler<TemperatureRequirementContext, TemperatureRangeRequirement> handler)
        {
            this._handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public async Task<PolicyResult> AuthorizeAsync(TemperatureRequirementContext context)
        {
            var results = await Task.WhenAll(this._temperatureRangeRequirements.Select(async x => await this._handler.HandleRequirementAsync(context, x)));

            var isAuthorized = await Task.FromResult(results.All(x => x));
            var unauthorizedReason = isAuthorized ? null : "The temperature is not warm.";

            return new PolicyResult(isAuthorized, unauthorizedReason);

        }
    }
}
