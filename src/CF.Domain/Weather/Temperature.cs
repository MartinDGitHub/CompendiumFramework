using System;
using System.Collections.Generic;

namespace CF.Domain.Weather
{
    public struct Temperature
    {
        private static IDictionary<TemperatureScale, IDictionary<TemperatureScale, Func<int, int>>> ToScaleConvertFuncByFromScale =
            new Dictionary<TemperatureScale, IDictionary<TemperatureScale, Func<int, int>>>
            {
                { TemperatureScale.Celsius, new Dictionary<TemperatureScale, Func<int, int>>
                    {
                        { TemperatureScale.Celsius, x => x },
                        { TemperatureScale.Farenheit, x => (int)Math.Round(x * 1.8M + 32, 0) },
                        { TemperatureScale.Kelvin, x => (int)Math.Round(x + 273.15M, 0) },
                    }
                },
                { TemperatureScale.Farenheit, new Dictionary<TemperatureScale, Func<int, int>>
                    {
                        { TemperatureScale.Celsius, x => (int)Math.Round((x - 32M) / 1.8M, 0) },
                        { TemperatureScale.Farenheit, x => x },
                        { TemperatureScale.Kelvin, x => (int)Math.Round((x + 459.67M) * (5M/9M), 0) },
                    }
                },
                { TemperatureScale.Kelvin, new Dictionary<TemperatureScale, Func<int, int>>
                    {
                        { TemperatureScale.Celsius, x => (int)Math.Round(x - 273.15M, 0) },
                        { TemperatureScale.Farenheit, x => (int)Math.Round(x * 1.8M - 459.67M, 0) },
                        { TemperatureScale.Kelvin, x => x },
                    }
                },
            };

        public TemperatureScale Scale { get; }

        public int Degrees { get; }

        public Temperature(int degrees)
        {
            this.Scale = TemperatureScale.Celsius;
            this.Degrees = degrees;
        }

        public Temperature(int value, TemperatureScale scale)
        {
            this.Scale = scale;
            this.Degrees = value;
        }

        public override string ToString()
        {
            return $"{this.Degrees} {TemperatureConstants.ScaleAbbrByScale[this.Scale]}";
        }

        public static Temperature Convert(Temperature fromTemperature, TemperatureScale toScale)
        {
            if (!ToScaleConvertFuncByFromScale.ContainsKey(fromTemperature.Scale))
            {
                throw new ArgumentException($"A conversion from a scale of [{fromTemperature.Scale}] is unsupported.");
            }

            var convertFuncByToScale = ToScaleConvertFuncByFromScale[fromTemperature.Scale];

            if (!convertFuncByToScale.ContainsKey(toScale))
            {
                throw new ArgumentException($"A conversion to a scale of [{toScale}] is unsupported.");
            }

            return new Temperature(convertFuncByToScale[toScale](fromTemperature.Degrees), toScale);
        }
    }
}
