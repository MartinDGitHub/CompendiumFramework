using CF.Common.Codes;
using System.Collections.Generic;
using Xunit;

namespace CF.Common.Test.Codes
{
    public class CodeBaseTests
    {
        private sealed class MockCode : CodeBase<MockCode, MockCode.MockId>
        {
            public static MockCode One = new MockCode(MockId.One, "1");
            public static MockCode Two = new MockCode(MockId.Two, "2");
            public static MockCode Three = new MockCode(MockId.Three, "3");

            public enum MockId
            {
                One,
                Two,
                Three,
            }

            private MockCode(MockId id, string value) : base(id, value)
            {
            }
        }

        [Fact]
        public void Codes_AccessBeforeStaticInitialization_Returned()
        {
            // Act
            var actualResult = MockCode.Codes["1"];

            // Assert
            Assert.Equal(actualResult, MockCode.One);
        }

        [Fact]
        public void Enumerable_EnumerateCodes_AllReturned()
        {
            // Arrange
            var expectedCodes = new HashSet<MockCode>
            {
                MockCode.One,
                MockCode.Two,
                MockCode.Three,
            };

            // Act
            var actualCodes = new HashSet<MockCode>();
            foreach (var mockCode in MockCode.Codes)
            {
                actualCodes.Add(mockCode);
            }

            // Assert
            Assert.False(actualCodes.IsProperSubsetOf(expectedCodes));
        }

        [Fact]
        public void CodesIdIndexer_AccessById_CodeFound()
        {
            // Arrange
            var expectedCode = MockCode.Two;

            // Act
            var actualCode = MockCode.Codes[MockCode.MockId.Two];

            // Assert
            Assert.Equal(expectedCode, actualCode);
        }

        [Fact]
        public void CodesValueIndexer_AccessByValue_CodeFound()
        {
            // Arrange
            var expectedCode = MockCode.Two;

            // Act
            var actualCode = MockCode.Codes["2"];

            // Assert
            Assert.Equal(expectedCode, actualCode);
        }

        [Fact]
        public void EqualsOperator_CheckForEquality_AreEqual()
        {
            // Assert
            #pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(MockCode.Two == MockCode.Two);
            #pragma warning restore CS1718 // Comparison made to same variable
        }

        [Fact]
        public void NotEqualsOperator_CheckForInequality_AreNotEqual()
        {
            // Assert
            Assert.True(MockCode.Two != MockCode.Three);
        }

        [Fact]
        public void EqualsOperator_CheckForEqualityWithId_AreEqual()
        {
            // Assert
            Assert.True(MockCode.Two == MockCode.MockId.Two);
            Assert.True(MockCode.MockId.Two == MockCode.Two);
        }

        [Fact]
        public void NotEqualsOperator_CheckForInequalityWithId_AreNotEqual()
        {
            // Assert
            Assert.True(MockCode.Two != MockCode.MockId.Three);
            Assert.True(MockCode.MockId.Three != MockCode.Two);
        }

        [Fact]
        public void EqualsOperator_CheckForEqualityWithValue_AreEqual()
        {
            // Assert
            Assert.True(MockCode.Two == "2");
            Assert.True("2" == MockCode.Two);
        }

        [Fact]
        public void NotEqualsOperator_CheckForInequalityWithValue_AreNotEqual()
        {
            // Assert
            Assert.True(MockCode.Two != "3");
            Assert.True("3" != MockCode.Two);
        }

        [Fact]
        public void ImplicitCastToValue_Cast_CastsExpectedValue()
        {
            // Act
            string actualValue = MockCode.Two;

            // Assert
            Assert.Equal("2", actualValue);
        }

        [Fact]
        public void ImplicitCastToId_Cast_CastsExpectedId()
        {
            // Act
            MockCode.MockId actualValue = MockCode.Two;

            // Assert
            Assert.Equal(MockCode.MockId.Two, actualValue);
        }

        [Fact]
        public void Equality_Switch_MatchesById()
        {
            // Act
            MockCode actualCode = null;
            var mockCode = MockCode.Two;
            switch (mockCode.Id)
            {
                case MockCode.MockId.One:
                    actualCode = MockCode.One;
                    break;
                case MockCode.MockId.Two:
                    actualCode = MockCode.Two;
                    break;
                case MockCode.MockId.Three:
                    actualCode = MockCode.Three;
                    break;
            }

            // Assert
            Assert.Equal(MockCode.Two, actualCode);
        }
    }
}
