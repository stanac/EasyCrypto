using EasyCrypto.Internal;

namespace EasyCrypto;

/// <summary>
/// Class for generating cryptographically secure passwords
/// </summary>
public class PasswordGenerator : IDisposable
{
    private readonly CryptoRandom _cr = new CryptoRandom();

    /// <summary>
    /// Default instance
    /// </summary>
    public static PasswordGenerator Default { get; } = new PasswordGenerator();
        
    /// <summary>
    /// Generates random password of 16 chars
    /// </summary>
    /// <returns>Random password</returns>
    public string Generate() => Generate(PasswordGenerationOptions.Default);

    /// <summary>
    /// Generates random password with desired length.
    /// </summary>
    /// <param name="length">Length of the password, default is 16, cannot be less than 4</param>
    /// <returns>Random password</returns>
    public string Generate(uint length) => Generate(PasswordGenerationOptions.Default.SetLength(length));

    /// <summary>
    /// Generates password using provided options. Options must be valid (check by calling <see cref="PasswordGenerationOptions.AreValid(out string)"/>).
    /// </summary>
    /// <param name="options">Options used for generating passwords</param>
    /// <returns>Random password</returns>
    public string Generate(PasswordGenerationOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        if (!options.AreValid(out string error))
        {
            throw new ArgumentException(error);
        }

        return GenerateInner(options.GetActuals());
    }

    private string GenerateInner(ActualPasswordGeneratorOptions options)
    {
        // at this point we expect options are validated and not null
        List<char> password = new List<char>();

        Action<string, int> generateGroup = (chars, length) =>
        {
            for (int i = 0; i < length; i++)
            {
                // ReSharper disable once AccessToModifiedClosure, intended
                password.Add(chars[_cr.NextInt(chars.Length)]);
            }
        };

        generateGroup(options.Numbers, options.NumbersLength);
        generateGroup(options.Upper,   options.UpperLength);
        generateGroup(options.Lower,   options.LowerLength);
        generateGroup(options.Symbols, options.SymbolsLength);

        password = ShuffleCharList(password);
        return new string(password.ToArray());
    }

    private List<char> ShuffleCharList(List<char> s)
    {
        for (int i = 0; i < s.Count / 2 || i == 0; i++)
        {
            for (int j = 0; j < s.Count; j++)
            {
                var temp = s[j];
                var nextPosition = _cr.NextInt(s.Count);
                s[j] = s[nextPosition];
                s[nextPosition] = temp;
            }
        }
        return s;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _cr.Dispose();
    }
}