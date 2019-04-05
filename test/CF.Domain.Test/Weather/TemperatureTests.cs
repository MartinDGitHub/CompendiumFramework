using Xunit;

namespace CF.Domain.Weather.Test
{
    public class TemperatureTests
    {
        [Fact]
        public void Convert_CelsiusToFarenheit_ConvertedAccurately()
        {
            // Arrange
            var temperature = new Temperature(42, TemperatureScale.Celsius);

            // Act
            var actual = Temperature.Convert(temperature, TemperatureScale.Farenheit);

            // Assert
            Assert.Equal(108, actual.Degrees);
        }

        [Fact]
        public void Convert_CelsiusToKelvin_ConvertedAccurately()
        {
            // Arrange
            var temperature = new Temperature(42, TemperatureScale.Celsius);

            // Act
            var actual = Temperature.Convert(temperature, TemperatureScale.Kelvin);

            // Assert
            Assert.Equal(315, actual.Degrees);
        }

        [Fact]
        public void Convert_FarenheitToCelsius_ConvertedAccurately()
        {
            // Arrange
            var temperature = new Temperature(42, TemperatureScale.Farenheit);

            // Act
            var actual = Temperature.Convert(temperature, TemperatureScale.Celsius);

            // Assert
            Assert.Equal(6, actual.Degrees);
        }

        [Fact]
        public void Convert_FarenheitToKelvin_ConvertedAccurately()
        {
            // Arrange
            var temperature = new Temperature(42, TemperatureScale.Farenheit);

            // Act
            var actual = Temperature.Convert(temperature, TemperatureScale.Kelvin);

            // Assert
            Assert.Equal(279, actual.Degrees);
        }

        [Fact]
        public void Convert_KelvinToCelsius_ConvertedAccurately()
        {
            // Arrange
            var temperature = new Temperature(42, TemperatureScale.Kelvin);

            // Act
            var actual = Temperature.Convert(temperature, TemperatureScale.Celsius);

            // Assert
            Assert.Equal(-231, actual.Degrees);
        }

        [Fact]
        public void Convert_KelvinToFarenheit_ConvertedAccurately()
        {
            // Arrange
            var temperature = new Temperature(42, TemperatureScale.Kelvin);

            // Act
            var actual = Temperature.Convert(temperature, TemperatureScale.Farenheit);

            // Assert
            Assert.Equal(-384, actual.Degrees);
        }
    }
}
