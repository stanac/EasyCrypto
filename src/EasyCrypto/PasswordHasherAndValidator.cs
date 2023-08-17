using EasyCrypto.Internal;

namespace EasyCrypto;

/// <summary>
/// PBKDF2 hasher and validator with hash and salt length of 64 bytes with embedded 64 bytes of random salt.
/// </summary>
public class PasswordHasherAndValidator
{
    private const int HashAndSaltLengthInBytes = 64;
    private const int MinIteration = 25_000;
    private readonly int _iterations;

    private static readonly FormattedBinaryData _binaryHashData = new FormattedBinaryData(90_002,
        typeof(int),     // hash and salt length
        typeof(int),     // iterations
        typeof(byte[]),  // salt
        typeof(byte[])   // hash
    );

    /// <summary>
    /// Constructor with 28K iterations
    /// </summary>
    public PasswordHasherAndValidator()
        : this(28_000)
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="iterations">Number of PBKDF2 iteration, minimum is 100000.</param>
    /// <exception cref="ArgumentException"></exception>
    public PasswordHasherAndValidator(int iterations)
    {
        if (iterations < MinIteration)
        {
            throw new ArgumentException($"Value cannot be less than {MinIteration}", nameof(iterations));
        }
            
        _iterations = iterations;
    }

    /// <summary>
    /// Hashes password with embedded random salt and returns it as base64 string
    /// </summary>
    /// <param name="password">Password to hash</param>
    /// <returns>Base64 string containing hash and salt that can be verified with <see cref="ValidatePassword(string,string)"/></returns>
    public string HashPasswordToString(string password) => Convert.ToBase64String(HashPassword(password));

    /// <summary>
    /// Hashes password with embedded random salt and returns it as byte array
    /// </summary>
    /// <param name="password">Password to hash</param>
    /// <returns>Byte array containing hash and salt that can be verified with <see cref="ValidatePassword(string,byte[])"/></returns>
    public byte[] HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("password cannot be null, empty or white space");
        }

        byte[] salt = RandomSalt();
        byte[] hash = Hash(password, salt, HashAndSaltLengthInBytes, _iterations);

        return _binaryHashData.ToBytes(HashAndSaltLengthInBytes, _iterations, salt, hash);
    }

    /// <summary>
    /// Validates password
    /// </summary>
    /// <param name="password">Password to validate</param>
    /// <param name="hashWithEmbeddedSalt">Password hash with embedded salt created by <see cref="HashPasswordToString"/> </param>
    /// <returns>Result of validation</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public PasswordHashValidationResult ValidatePassword(string password, string hashWithEmbeddedSalt)
    {
        byte[] hashBytes;
        try
        {
            hashBytes = Convert.FromBase64String(hashWithEmbeddedSalt);
        }
        catch
        {
            throw new InvalidOperationException("Hash is not valid base64");
        }

        return ValidatePassword(password, hashBytes);
    }

    /// <summary>
    /// Validates password
    /// </summary>
    /// <param name="password">Password to validate</param>
    /// <param name="hashWithEmbeddedSalt">Password hash with embedded salt created by <see cref="HashPassword"/> </param>
    /// <returns>Result of validation</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public PasswordHashValidationResult ValidatePassword(string password, byte[] hashWithEmbeddedSalt)
    {
        object[] result = _binaryHashData.Read(hashWithEmbeddedSalt);

        int length = (int)result[0];
        int iterations = (int)result[1];
        byte[] salt = (byte[]) result[2];
        byte[] hashBytes = (byte[]) result[3];

        byte[] newHash = Hash(password, salt, length, iterations);

        bool isValid = InternalDataTools.CompareByteArrays(hashBytes, newHash);

        if (!isValid)
        {
            return PasswordHashValidationResult.NotValid;
        }

        if (iterations < _iterations || length < HashAndSaltLengthInBytes)
        {
            return PasswordHashValidationResult.ValidShouldRehash;
        }

        return PasswordHashValidationResult.Valid;
    }

    private static byte[] RandomSalt() => CryptoRandom.Default.NextBytes(HashAndSaltLengthInBytes);

    internal static byte[] HashOld(string password, byte[] salt, int length, int iterations)
    {
#pragma warning disable SYSLIB0041
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
        pbkdf2.IterationCount = iterations;
        return pbkdf2.GetBytes(length);
#pragma warning restore SYSLIB0041
    }

    // this implementation uses new non obsolete Rfc2898DeriveBytes ctor
    internal static byte[] Hash(string password, byte[] salt, int length, int iterations)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA1);
        return pbkdf2.GetBytes(length);
    }
}