using System;
using System.Security.Cryptography;

namespace Solitaire.Utils;

public static class PlatformProviders
{
    public static double NextRandomDouble()
    {
        var nextULong = BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(sizeof(ulong)));

        return (nextULong >> 11) * (1.0 / (1ul << 53));
    }
}