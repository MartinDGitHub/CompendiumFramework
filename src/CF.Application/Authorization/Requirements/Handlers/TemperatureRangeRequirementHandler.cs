using CF.Application.Authorization.Requirements.Contexts;
using CF.Common.Authorization.Requirements.Handlers;
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

        public async Task<bool> HandleRequirementAsync(TemperatureRequirementContext context, TemperatureRangeRequirement requirement)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var isAuthorized = await Task.FromResult(context.Temperature >= requirement.MinTemperature && context.Temperature < requirement.MaxTemperature);

            if (!isAuthorized)
            {
                this._logger.Information($"Temperature of [{context.Temperature}] is unauthorized for required temperature range [{requirement.MinTemperature}] [{requirement.MaxTemperature}]");
            }

            return isAuthorized;
        }
    }
}
