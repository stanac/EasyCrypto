namespace EasyCrypto
{
    /// <summary>
    /// This options are actually used for generating passwords.
    /// This options are used to set actual length of different char groups (Upper, Lower, Numbers and Symbols)
    /// because in other options we are using min values.
    /// </summary>
    internal class ActualPasswordGeneratorOptions
    {
        public int Length { get; set; }
        public string Upper { get; set; }
        public string Lower { get; set; }
        public string Numbers => "0123456789";
        public string Symbols { get; set; }
        public int UpperLength { get; set; }
        public int LowerLength { get; set; }
        public int NumbersLength { get; set; }
        public int SymbolsLength { get; set; }
        
    }
}
