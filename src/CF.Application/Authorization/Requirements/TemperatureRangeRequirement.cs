using CF.Common.Authorization.Requirements;
using System;

namespace CF.Application.Authorization.Requirements
{
    internal class TemperatureRangeRequirement : IRequirement
    {
        /// <summary>
        /// Gets the minimum temperature (inclusive).
        /// </summary>
        public int MinTemperature { get; }

        /// <summary>
        /// Gets the maximum temperature (exclusive).
        /// </summary>
        public int MaxTemperature { get; }

        public TemperatureRangeRequirement(int minTemperature, int maxTemperature)
        {
            if (minTemperature > maxTemperature)
            {
                throw new ArgumentOutOfRangeException(nameof(minTemperature), $"The min temperature of [{minTemperature}] is greater than the max temperature of [{maxTemperature}].");
            }

            this.MinTemperature = minTemperature;
            this.MaxTemperature = maxTemperature;
        }
    }
}
