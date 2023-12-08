namespace Revrs;

/// <summary>
/// Byte order mark (BoM) defined as a <see langword="big-endian"/> unsigned word (<see langword="ushort"/>)
/// </summary>
public enum Endianness : ushort
{
    Big = 0xFEFF,
    Little = 0xFFFE,
}

public static class RevrsPrimitives
{
    /// <summary>
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the system <see cref="Endianness"/> matches the provided <paramref name="endianness"/>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool MatchesSystemEndianness(this Endianness endianness)
    {
        return BitConverter.IsLittleEndian == (endianness == Endianness.Little);
    }
}
