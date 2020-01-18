using CF.Application.Authorization.Requirements;
using CF.Application.Authorization.Requirements.Contexts;
using CF.Application.Services.Weather;
using CF.Common.Authorization.Policies;
using CF.Common.Authorization.Requirements;
using System;
using System.Threading.Tasks;

namespace CF.Application.Authorization.Policies.Weather
{
    internal class NormalTemperaturePolicy : INormalTemperaturePolicy
    {
        private IWeatherStatisticsService _weatherStatisticsService;
        private readonly IRequirementHandler<TemperatureRequirementContext, TemperatureRangeRequirement> _handler;

        public NormalTemperaturePolicy(IWeatherStatisticsService weatherStatisticsService, IRequirementHandler<TemperatureRequirementContext, TemperatureRangeRequirement> handler)
        {
            this._weatherStatisticsService = weatherStatisticsService ?? throw new ArgumentNullException(nameof(weatherStatisticsService));
            this._handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public async Task<PolicyResult> AuthorizeAsync(TemperatureRequirementContext context)
        {
            var temperatureRange = this._weatherStatisticsService.GetNormalTemperatureRange(DateTime.Now);
            var requirement = new TemperatureRangeRequirement(temperatureRange);
            var result = await this._handler.HandleRequirementAsync(context, requirement).ConfigureAwait(false);
            var isAuthorized = result.IsMet;
            var unauthorizedReason = isAuthorized ? null : "The temperature is not within the normal range.";

            return new PolicyResult(this, isAuthorized, new string[] { unauthorizedReason, result.UnmetMessage }, unauthorizedReason);

        }
    }
}
