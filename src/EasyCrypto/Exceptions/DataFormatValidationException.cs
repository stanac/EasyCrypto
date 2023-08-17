namespace EasyCrypto.Exceptions;

/// <summary>
/// Data Format Validation Exception
/// </summary>
/// <seealso cref="System.Exception" />
public class DataFormatValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataFormatValidationException"/> class.
    /// </summary>
    public DataFormatValidationException() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="DataFormatValidationException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DataFormatValidationException(string message) : base(message) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="DataFormatValidationException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="error">The error.</param>
    public DataFormatValidationException(string message, DataValidationErrors error) : base(message) { ValidationError = error; }
    /// <summary>
    /// Initializes a new instance of the <see cref="DataFormatValidationException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner.</param>
    public DataFormatValidationException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// Gets or sets the validation error.
    /// </summary>
    /// <value>
    /// The validation error.
    /// </value>
    public DataValidationErrors? ValidationError { get; set; }

    /// <summary>
    /// Types of validation errors
    /// </summary>
    public enum DataValidationErrors
    {
        /// <summary>
        /// data is too short
        /// </summary>
        DataIsTooShort,
        /// <summary>
        /// invalid magic number
        /// </summary>
        InvalidMagicNumber,
        /// <summary>
        /// unsupported data version
        /// </summary>
        UnsupportedDataVersion,
        /// <summary>
        /// data integrity validation error
        /// </summary>
        DataIntegrityValidationError,
        /// <summary>
        /// key check value validation error
        /// </summary>
        KeyCheckValueValidationError
    }
}