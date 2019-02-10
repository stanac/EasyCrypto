using System.Linq;
using Xunit;

namespace EasyCrypto.Tests
{
    public class TokenGeneratorTests
    {
        [Fact]
        public void GenerateToken_DefinedChars_DoesNotContainOtherChars()
        {
            var tokenGen = new TokenGenerator("1234567890");
            var token = tokenGen.GenerateToken(260);

            int numberOfNonNumericChars = token.Count(c => !char.IsNumber(c));

            Assert.Equal(0, numberOfNonNumericChars);
        }

        [Fact]
        public void GenerateToken_GeneratesTokenFromDifferentChars()
        {
            var tokenGen = new TokenGenerator();
            var token = tokenGen.GenerateToken(1000);

            int numberOfUniqueChars = token.Distinct().Count();

            Assert.True(numberOfUniqueChars > 20); // we cannot actually test this in correct way because it's random
        }
    }
}
