using CF.Domain.Weather;
using Xunit;

namespace CF.Domain.Test.Weather
{
    public class TemperatureRangeTests
    {
        [Theory]
        [InlineData(15, 10, 20)]
        [InlineData(10, 10, 20)]
        [InlineData(19, 10, 20)]
        public void IsInRange_DegreesBetweenMinAndMax_FoundToBeInRange(int degrees, int min, int max)
        {
            // Arrange
            var scale = TemperatureScale.Celsius;
            var temperature = new Temperature(degrees, scale);
            var temperatureRange = new TemperatureRange(scale, min, max);

            // Act
            var actual = temperatureRange.IsInRange(temperature);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(9, 10, 20)]
        [InlineData(20, 10, 20)]
        [InlineData(21, 10, 20)]
        public void IsInRange_DegreesBetweenMinAndMax_NotFoundToBeInRange(int degrees, int min, int max)
        {
            // Arrange
            var scale = TemperatureScale.Celsius;
            var temperature = new Temperature(degrees, scale);
            var temperatureRange = new TemperatureRange(scale, min, max);

            // Act
            var actual = temperatureRange.IsInRange(temperature);

            // Assert
            Assert.False(actual);
        }
    }
}
