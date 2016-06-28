using System;

namespace EasyCrypto
{
    [Flags]
    internal enum CryptoContainerFlags
    {
        HasSalt = 0x01
        // HasIv   = 0x02
    }
}
