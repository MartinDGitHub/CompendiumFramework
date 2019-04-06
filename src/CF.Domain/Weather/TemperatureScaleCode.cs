using CF.Common.Codes;

namespace CF.Domain.Weather
{
    public sealed class TemperatureScaleCode : CodeBase<TemperatureScaleCode, TemperatureScale>
    {
        public static TemperatureScaleCode Celsius = new TemperatureScaleCode(TemperatureScale.Celsius, "C", "Temperature at which water freezes at 0 and boils at 100 under standard conditions.");
        public static TemperatureScaleCode Farenheit = new TemperatureScaleCode(TemperatureScale.Farenheit, "F", "Temperature at which water freezes at 32 and boils at 212.");
        public static TemperatureScaleCode Kelvin = new TemperatureScaleCode(TemperatureScale.Kelvin, "K", "Temperature with absolute zero as zero.");

        public string Description { get; }

        // Prevent external construction of new codes, this class is the definition of the valid set.
        private TemperatureScaleCode(TemperatureScale id, string value, string description) : base(id, value)
        {
            this.Description = description;
        }
    }
}
