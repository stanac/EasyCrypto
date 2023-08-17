using EasyCrypto.Internal;

namespace EasyCrypto;

/// <summary>
/// Id generator that has two mandatory parts (time base part, random part) and
/// one optional part (fixed part), 
/// generated id templateId without hyphens is {timePart}{fixedPart}{randomPart}
/// generated id templateId wit hyphens is {timePart}-{fixedPart}-{randomPart}
/// </summary>
public class IdGenerator
{
    /// <summary>
    /// Default instance of Id generator with FastRandom = true, FixedPart = "", RandomPartLength = 6 and AddHyphens = false
    /// </summary>
    public static IdGenerator Default { get; } = new IdGenerator();

    /// <summary>
    /// Fixed part that is set in the middle of the generated Id
    /// </summary>
    public string FixedPart { get; }

    /// <summary>
    /// If true, System.Random is used for random part, if false EasyCrypto.CryptoRandom is used
    /// </summary>
    public bool FastRandom { get; }

    private int _randomPartLength = 6;

    /// <summary>
    /// Length of random part, default value is 6, must not be less than 4 or greater than 100
    /// </summary>
    public int RandomPartLength
    {
        get => _randomPartLength;
        set
        {
            if (value < 4) throw new ArgumentOutOfRangeException(nameof(value), "Random part length cannot be less than 4");
            if (value > 100) throw new ArgumentOutOfRangeException(nameof(value), "Random part length cannot be greater than 100");
            _randomPartLength = value;
        }
    }

    /// <summary>
    /// If true hyphens (-) are added between parts
    /// </summary>
    public bool AddHyphens { get; set; }

    /// <summary>
    /// Default constructor, FixedPart = "", FastRandom = true
    /// </summary>
    public IdGenerator() : this("", true)
    {
    }

    /// <summary>
    /// Constructor to accept boolean value telling the generator to use or not to use fast random
    /// </summary>
    /// <param name="fastRandom">If true System.Random is used, otherwise EasyCrypto.CryptoRandom is used</param>
    public IdGenerator(bool fastRandom): this("", fastRandom)
    {
    }

    /// <summary>
    /// Constructor to accept boolean value telling the generator to use or not to use fast random and
    /// fixed part of generated id
    /// </summary>
    /// <param name="fixedPart">Fixed part to set in middle of generated id</param>
    /// <param name="fastRandom">If true System.Random is used, otherwise EasyCrypto.CryptoRandom is used</param>
    public IdGenerator(string fixedPart, bool fastRandom)
    {
        FixedPart = (fixedPart ?? "").Trim();
        FastRandom = fastRandom;
    }

    /// <summary>
    /// Generates new id string where for time part current UTC time is used
    /// </summary>
    /// <returns>String, generated id</returns>
    public string NewId() => NewId(DateTime.UtcNow);

    /// <summary>
    /// Generates new id using specified time
    /// Warning: when using this override, make sure to specify very precise time, including milliseconds
    /// </summary>
    /// <param name="time">Time to use in id generation</param>
    /// <returns>String, generated id</returns>
    public string NewId(DateTime time)
    {
        if (AddHyphens)
        {
            if (FixedPart.Length > 0)
            {
                return $"{GetTimePart(time)}-{FixedPart}-{GetRandomPart()}";
            }

            return $"{GetTimePart(time)}-{GetRandomPart()}";
        }

        return GetTimePart(time) + FixedPart + GetRandomPart();
    }

    private string GetTimePart(DateTime t)
    {
        long offset = DateTimeOriginOffset.GetOffset(t);
        return SystemStringBase55Converter.ToString(offset, 8);
    }

    private string GetRandomPart()
    {
        char[] ret = new char[RandomPartLength];

        int index;
        if (FastRandom)
        {
            for (int i = 0; i < ret.Length; i++)
            {
                index = ThreadSafeRandom.Default.Next(SystemStringBase55Converter.Charset.Length);
                ret[i] = SystemStringBase55Converter.Charset[index];
            }
        }
        else
        {
            for (int i = 0; i < ret.Length; i++)
            {
                index = CryptoRandom.Default.NextInt(SystemStringBase55Converter.Charset.Length);
                ret[i] = SystemStringBase55Converter.Charset[index];
            }
        }

        return new string(ret);
    }
}