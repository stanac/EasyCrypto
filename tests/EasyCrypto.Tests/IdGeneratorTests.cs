using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EasyCrypto.Tests;

public class IdGeneratorTests
{
    [Fact]
    public void GeneratorWithFastRandom_GeneratesRandomId()
    {
        var idGen = new IdGenerator(true);
        idGen.AddHyphens = true;

        List<string> ids = new List<string>();
        for (int i = 0; i < 10; i ++)
        {
            string id = idGen.NewId();
            ids.Add(id);
            Assert.NotNull(id);
        }

        int distinctCount = ids.Distinct().Count();
        Assert.Equal(ids.Count, distinctCount);
    }

    [Fact]
    public void GeneratorWithSlowRandom_GeneratesRandomId()
    {
        var idGen = new IdGenerator(false);
        idGen.AddHyphens = true;

        List<string> ids = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            string id = idGen.NewId();
            ids.Add(id);
            Assert.NotNull(id);
        }

        int distinctCount = ids.Distinct().Count();
        Assert.Equal(ids.Count, distinctCount);
    }

    [Fact]
    public void AddHyphensTrue_GeneratesIdWithHyphens()
    {
        IdGenerator idGen = new IdGenerator
        {
            AddHyphens = true
        };

        string id = idGen.NewId();

        Assert.Contains("-", id);
    }

    [Fact]
    public void AddHyphensFalse_GeneratesIdWithoutHyphens()
    {
        string id = IdGenerator.Default.NewId();

        Assert.DoesNotContain("-", id);
    }

    [Fact]
    public void DateTimeYear4000PartTakes8Chars()
    {
        IdGenerator idGen = new IdGenerator
        {
            AddHyphens = true
        };

        var id = idGen.NewId(new DateTime(4000, 2, 1));

        string timePart = id.Split('-')[0];
        Assert.Equal(8, timePart.Length);
    }

    [Fact]
    public void DateTimeYear6000PartTakes9Chars()
    {
        IdGenerator idGen = new IdGenerator
        {
            AddHyphens = true
        };

        var id = idGen.NewId(new DateTime(6000, 2, 1));

        string timePart = id.Split('-')[0];
        Assert.Equal(9, timePart.Length);
    }

    [Fact]
    public void FixedPartNotSet_WithHyphens_HasOneHyphens()
    {
        IdGenerator idGen = new IdGenerator("", true);
        idGen.AddHyphens = true;

        var id = idGen.NewId();
        int count = id.Count(x => x == '-');
        Assert.Equal(1, count);
    }

    [Fact]
    public void FixedPartSet_WithHyphens_HasTwoHyphens()
    {
        IdGenerator idGen = new IdGenerator("ABC", true);
        idGen.AddHyphens = true;

        var id = idGen.NewId();
        int count = id.Count(x => x == '-');
        Assert.Equal(2, count);
    }

    [Fact]
    public void FixedPartSet_GeneratedIdContainsFixedPart()
    {
        IdGenerator idGen = new IdGenerator("ABC", true);
        idGen.AddHyphens = true;

        var id = idGen.NewId();
        Assert.Contains("-ABC-", id);
    }

    [Fact]
    public void RandomPartLengthSet_GeneratesRandomPartOfEqualLength()
    {
        int[] lengths = { 8, 16, 32, 64, 70 };

        foreach (var length in lengths)
        {
            IdGenerator idGen = new IdGenerator();
            idGen.AddHyphens = true;
            idGen.RandomPartLength = length;

            var id = idGen.NewId();
            int randomPartLength = id.Split('-')[1].Length;

            Assert.Equal(idGen.RandomPartLength, randomPartLength);
        }
    }
}