using CF.Application.Authorization.Requirements.Contexts;
using CF.Common.Authorization.Requirements;
using CF.Common.Logging;
using System;
using System.Threading.Tasks;

namespace CF.Application.Authorization.Requirements.Handlers
{
    internal class TemperatureRangeRequirementHandler : IRequirementHandler<TemperatureRequirementContext, TemperatureRangeRequirement>
    {
        private readonly ILogger _logger;

        public TemperatureRangeRequirementHandler(ILogger<TemperatureRangeRequirementHandler> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<RequirementResult> HandleRequirementAsync(TemperatureRequirementContext context, TemperatureRangeRequirement requirement)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var isMet = requirement.Range.IsInRange(context.Temperature);

            return Task.FromResult(new RequirementResult(isMet, 
                isMet
                ? null 
                : $"Temperature of [{context.Temperature}] is not within range of: [{requirement.Range.Min}](inclusive) to [{requirement.Range.Max}](exclusive).")
            );
        }
    }
}
