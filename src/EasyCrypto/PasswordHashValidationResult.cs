namespace EasyCrypto
{
    /// <summary>
    /// Password hash validation result
    /// </summary>
    public enum PasswordHashValidationResult
    {
        /// <summary>
        /// Hash is valid
        /// </summary>
        Valid = 1,

        /// <summary>
        /// Hash is not valid
        /// </summary>
        NotValid = 2,

        /// <summary>
        /// Hash is valid and password is required to be rehashed with new settings
        /// </summary>
        ValidShouldRehash = 3
    }
}
