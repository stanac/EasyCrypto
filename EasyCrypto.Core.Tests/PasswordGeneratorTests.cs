using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EasyCrypto.Tests
{
    public class PasswordGeneratorTests
    {
        private const int NumberOfRunsPerTest = 5;

        [Fact]
        public void DefaultPasswordGeneratorGroupsAreOk() => AssertPasswordGroups(
            PasswordGenerationOptions.Default
            );

        [Fact]
        public void NoNumbersPasswordGeneratorGroupsArOk() => AssertPasswordGroups(
            PasswordGenerationOptions.Default
                .SetMinNumbers(0)
            );

        [Fact]
        public void NoNumbersNoSymbolsPasswordGeneratorGroupsArOk() => AssertPasswordGroups(
            PasswordGenerationOptions.Default
                .SetMinNumbers(0)
                .SetMinSymbols(0)
            );

        [Fact]
        public void NoLowerCasePasswordGeneratorGroupsArOk() => AssertPasswordGroups(
            PasswordGenerationOptions.Default
                .SetMinLowerCase(0)
            );

        [Fact]
        public void NoUpperCasePasswordGeneratorGroupsArOk() => AssertPasswordGroups(
            PasswordGenerationOptions.Default
                .SetMinUpperCase(0)
            );

        [Fact]
        public void NoSymbolsPasswordGeneratorGroupsArOk() => AssertPasswordGroups(
            PasswordGenerationOptions.Default
                .SetMinSymbols(0)
            );

        [Fact]
        public void ShortPasswordGroupsAreOk() => AssertPasswordGroups(
            PasswordGenerationOptions.Default
                .SetLength(4)
            );

        [Fact]
        public void SymbolsOnlyPasswordGeneratorGroupsArOk() => AssertPasswordGroups(
            PasswordGenerationOptions.Default
                .SetMinNumbers(0)
                .SetMinLowerCase(0)
                .SetMinUpperCase(0)
            );

        [Fact]
        public void CustomSymbolsPasswordGeneratorIsOk() => AssertPasswordGroups(
            PasswordGenerationOptions.Default
                .SetMinNumbers(0)
                .SetMinLowerCase(0)
                .SetMinUpperCase(0)
                .UseSymbols("ab")
            );

        [Fact]
        public void AssertPasswordsAreShuffled()
        {
            int inRangeCount = 0;
            var options = PasswordGenerationOptions.Default;
            using (PasswordGenerator pg = new PasswordGenerator())
            {
                string pass;
                for (int i = 0; i < NumberOfRunsPerTest; i++)
                {
                    pass = pg.Generate(options);
                    if (IsSinglePasswordInRange(pass, options))
                    {
                        inRangeCount++;
                    }
                }
            }
            Assert.True((double)inRangeCount < 0.95 * NumberOfRunsPerTest);
        }

        private void AssertPasswordGroups(PasswordGenerationOptions options) =>
            AssertPasswords(options, AssertSinglePasswordGroups);

        private void AssertPasswords(PasswordGenerationOptions options, Action<string, PasswordGenerationOptions> assertAction)
        {
            using (PasswordGenerator pg = new PasswordGenerator())
            {
                string pass;
                for (int i = 0; i < NumberOfRunsPerTest; i++)
                {
                    pass = pg.Generate(options);
                    assertAction(pass, options);
                }
            }
        }

        private void AssertSinglePasswordGroups(string password, PasswordGenerationOptions options)
        {
            Assert.True(password.Length == (int)options.Length, "Length is not ok");
            
            int numberOfUpper = password.Count(options.ValidUpperCase.Contains);
            int numberOfLower = password.Count(options.ValidLowerCase.Contains);
            int numberOfSymbols = password.Count(options.ValidSymbols.Contains);
            int numberOfNumbers = password.Count("0123456789".Contains);

            Assert.True(numberOfLower >= options.MinLowerCase, "Number of lower case is less than minimum");
            Assert.True(numberOfUpper >= options.MinUpperCase, "Number of upper case is less than minimum");
            Assert.True(numberOfSymbols >= options.MinSymbols, "Number of symbols is less than minimum");
            Assert.True(numberOfNumbers >= options.MinNumbers, "Number of numbers is less than minimum");
        }

        private bool IsSinglePasswordInRange(string password, PasswordGenerationOptions options)
        {
            Func<IEnumerable<int>, bool> areIndexesInRange = (numbers) =>
            {
                if (numbers.Count() == 0)
                {
                    return false;
                }

                if (numbers.Count() != numbers.Distinct().Count())
                {
                    throw new ArgumentException("numbers are not unique");
                }
                return numbers.Max() - numbers.Min() == numbers.Count() - 1;
            };

            Func<string, List<int>> getGroupIndexes = (string group) =>
            {
                List<int> indexes = new List<int>();
                for (int i = 0; i < password.Length; i++)
                {
                    if (group.Contains(password[i]))
                    {
                        indexes.Add(i);
                    }
                }
                return indexes;
            };

            Func<string, bool> isGroupInRange = (group) =>
            {
                var indexes = getGroupIndexes(group);
                return areIndexesInRange(indexes);
            };

            bool lowerAreInRange = isGroupInRange(options.ValidLowerCase);
            bool upperAreInRange = isGroupInRange(options.ValidUpperCase);
            bool symbolsAreIsInRange = isGroupInRange(options.ValidSymbols);
            bool numbersAreInRange = isGroupInRange("0123456789");

            return lowerAreInRange && upperAreInRange && symbolsAreIsInRange && numbersAreInRange;
        }
    }
}
