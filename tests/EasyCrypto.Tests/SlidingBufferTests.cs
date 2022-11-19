using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCrypto.Internal;
using Xunit;

namespace EasyCrypto.Tests;

public class SlidingBufferTests
{
    private readonly SlidingBuffer _sut;

    public SlidingBufferTests()
    {
        byte currentGenerated = 0;

        _sut = new SlidingBuffer(t =>
        {
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = currentGenerated;
                unchecked { currentGenerated++; }
            }
        });
    }

    [Fact]
    public void BufferCopiesDataInOrder()
    {
        List<byte[]> data = new List<byte[]>();
        data.Add(new byte[4]);
        data.Add(new byte[5]);
        data.Add(new byte[6]);
        data.Add(new byte[7]);
        data.Add(new byte[8]);
        data.Add(new byte[9]);

        foreach (var d in data)
        {
            _sut.GetData(d);
        }

        List<byte> joined = data.SelectMany(x => x).ToList();
        int distinctCount = joined.Distinct().Count();

        Assert.Equal(joined.Count, distinctCount);
    }

    [Fact]
    public void ParallelBufferCopiesDataInOrder()
    {
        List<byte[]> data = new List<byte[]>();
        data.Add(new byte[4]);
        data.Add(new byte[5]);
        data.Add(new byte[6]);
        data.Add(new byte[7]);
        data.Add(new byte[8]);
        data.Add(new byte[9]);

        Parallel.ForEach(data, d => _sut.GetData(d));

        List<byte> joined = data.SelectMany(x => x).ToList();
        int distinctCount = joined.Distinct().Count();

        Assert.Equal(joined.Count, distinctCount);
    }
}