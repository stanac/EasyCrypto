using EasyCrypto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EasyCrypto.Tests
{
    public class CryptoRandomTests
    {
        private const int _itterations = 100;

        [Fact]
        public void DoubleIsBetweenZeroAndOne()
        {
            for (int i = 0; i < _itterations; i++)
            {
                double d = CryptoRandom.NextDoubleStatic();
                Assert.True(d >= 0 && d <= 1);
            }
        }

        [Fact]
        public void MinMustBeLessThanMax()
        {
            int[] a = new int[0];
            try
            {
                CryptoRandom.NextIntStatic(5, 5);
            }
            catch (ArgumentException)
            {
                Assert.True(true);
                return;
            }
            Assert.True(false);
        }

        [Fact]
        public void NextIntIsNonNegative()
        {
            for (int i = 0; i < _itterations; i++)
            {
                int a = CryptoRandom.NextIntStatic();
                Assert.True(a >= 0);
            }
        }

        [Fact]
        public void NextIntValueIsLessThanMaxValue()
        {
            const int max = 10;
            for (int i = 0; i < _itterations; i++)
            {
                int a = CryptoRandom.NextIntStatic(max);
                Assert.True(a < max);
            }
        }
    }
}
