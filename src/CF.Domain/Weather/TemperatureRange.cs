using System;

namespace CF.Domain.Weather
{
    public struct TemperatureRange
    {
        /// <summary>
        /// Gets the temperature scale that the range is for.
        /// </summary>
        public TemperatureScale Scale { get; }

        /// <summary>
        /// Gets the minimum value for the range (inclusive).
        /// </summary>
        public Temperature Min { get; }

        /// <summary>
        /// Gets the maximum value for the range (exclusive).
        /// </summary>
        public Temperature Max { get; }

        public TemperatureRange(TemperatureScale scale, int min, int max)
        {
            if (min >= max)
            {
                throw new ArgumentOutOfRangeException($"The min temperature of [{min}] must be less than the max temperature [{max}].");
            }

            this.Scale = scale;
            this.Min = new Temperature(min, scale);
            this.Max = new Temperature(max, scale);
        }

        public bool IsInRange(Temperature temperature)
        {
            var convertedTemperature = Temperature.Convert(temperature, this.Scale);

            return (convertedTemperature.Degrees >= this.Min.Degrees && convertedTemperature.Degrees < this.Max.Degrees);
        }
    }
}
