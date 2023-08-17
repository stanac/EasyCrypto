using EasyCrypto.Exceptions;
using static EasyCrypto.Exceptions.DataFormatValidationException;

namespace EasyCrypto;

/// <summary>
/// Results of validating encrypted data
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether [key/password is valid].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [key/password is valid]; otherwise, <c>false</c>.
    /// </value>
    public bool KeyIsValid { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether [data integrity is valid].
    /// </summary>
    /// <value>
    /// <c>true</c> if [data integrity is valid]; otherwise, <c>false</c>.
    /// </value>
    public bool DataIntegrityIsValid { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether [data format is valid].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [data format is valid]; otherwise, <c>false</c>.
    /// </value>
    public bool DataFormatIsValid { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether [data format version is valid].
    /// </summary>
    /// <value>
    /// <c>true</c> if [data format version is valid]; otherwise, <c>false</c>.
    /// </value>
    public bool DataFormatVersionIsValid { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether [data format version is exact].
    /// </summary>
    /// <value>
    /// <c>true</c> if [data format version is exact]; otherwise, <c>false</c>.
    /// </value>
    public bool DataFormatVersionIsExact { get; internal set; }

    /// <summary>
    /// Returns true if everything is valid.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
    /// </value>
    public bool IsValid => KeyIsValid && DataFormatIsValid && DataIntegrityIsValid && DataFormatVersionIsValid;

    /// <summary>
    /// Gets the exception to throw.
    /// </summary>
    /// <value>
    /// The exception to throw.
    /// </value>
    internal DataFormatValidationException ExceptionToThrow { get; private set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>
    /// The error message.
    /// </value>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the type of the error.
    /// </summary>
    /// <value>
    /// The type of the error.
    /// </value>
    public DataValidationErrors? ErrorType { get; set; }

    /// <summary>
    /// Sets the exception.
    /// </summary>
    /// <param name="error">The error.</param>
    internal void SetException(DataValidationErrors error)
    {
        ErrorType = error;
        string message = "Unknown error";
        switch (error)
        {
            case DataValidationErrors.DataIntegrityValidationError:
                message = "Data integrity is compromised.";
                break;
            case DataValidationErrors.DataIsTooShort:
                message = "Data does not appear to be encrypted by EasyCrypto";
                break;
            case DataValidationErrors.InvalidMagicNumber:
                message = "Data does not appear to be encrypted by EasyCrypto";
                break;
            case DataValidationErrors.KeyCheckValueValidationError:
                message = "Key/IV or password is not valid";
                break;
            case DataValidationErrors.UnsupportedDataVersion:
                message = "Data is encrypted by newer version of the EasyCrypto";
                break;
        }
        ExceptionToThrow = new DataFormatValidationException(message, error);
        ErrorMessage = message;
    }
}