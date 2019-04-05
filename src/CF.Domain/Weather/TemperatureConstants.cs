using System.Collections.Generic;

namespace CF.Domain.Weather
{
    public static class TemperatureConstants
    {
        public static IDictionary<TemperatureScale, string> ScaleAbbrByScale = new Dictionary<TemperatureScale, string>
        {
            { TemperatureScale.Celsius, "C" },
            { TemperatureScale.Farenheit, "F" },
            { TemperatureScale.Kelvin, "K" },
        };
    }
}
