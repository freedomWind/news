using System.Security.Cryptography;

namespace NewsFramework.Media.Api.Utilities;

public static class IdGenerator
{
    public static string NewId(string prefix)
    {
        Span<byte> bytes = stackalloc byte[12];
        RandomNumberGenerator.Fill(bytes);
        return prefix + "_" + Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
