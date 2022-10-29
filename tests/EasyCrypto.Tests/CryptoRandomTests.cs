using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EasyCrypto.Tests;

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

    [Fact]
    public void NextInt_WithBuffer_DoesntBreakGeneratesUniqueValues()
    {
        var data = Enumerable.Range(1, 2000).ToList();
        data = RandomizeList(data);

        ParallelOptions o = new ParallelOptions
        {
            MaxDegreeOfParallelism = data.Count
        };

        var rng = new CryptoRandom(true);

        Parallel.ForEach(data, d => rng.NextBytes((uint)d));

        var l = data.GroupBy(x => x).OrderBy(x => x.Count()).ToList();
        int maxRepats = l.First().Count();

        Assert.True(maxRepats < 20);
    }

    private List<T> RandomizeList<T>(List<T> list)
    {
        var temp = list.ToList();
        List<T> result = new List<T>();

        var rng = new CryptoRandom();

        while (temp.Any())
        { 
            int index = rng.NextInt(temp.Count);

            result.Add(temp[index]);
            temp.RemoveAt(index);
        }

        return result;
    }
}