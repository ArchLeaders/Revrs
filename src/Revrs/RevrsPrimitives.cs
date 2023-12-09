using System.Numerics;

namespace Revrs;

/// <summary>
/// Byte order mark (BoM) defined as a <see langword="big-endian"/> unsigned word (<see langword="ushort"/>).
/// </summary>
public enum Endianness : ushort
{
    /// <summary>
    /// Big Endian Byte order.
    /// </summary>
    Big = 0xFEFF,

    /// <summary>
    /// Little Endian Byte Order.
    /// </summary>
    Little = 0xFFFE,
}

/// <summary>
/// Primitive extension methods used by <see cref="Revrs"/>.
/// </summary>
public static class RevrsPrimitives
{
    /// <summary>
    /// Compare the system endianness with the provided <paramref name="endianness"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the system <see cref="Endianness"/> matches the provided <paramref name="endianness"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSystemEndianness(this Endianness endianness)
    {
        return BitConverter.IsLittleEndian == (endianness == Endianness.Little);
    }

    /// <summary>
    /// Compare the system endianness with the provided <paramref name="endianness"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the system <see cref="Endianness"/> <i>does not match</i> the provided <paramref name="endianness"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotSystemEndianness(this Endianness endianness)
    {
        return BitConverter.IsLittleEndian == (endianness == Endianness.Big);
    }

    /// <summary>
    /// Align <paramref name="value"/> up to <paramref name="size"/> and return the result.
    /// </summary>
    /// <typeparam name="T">The integral type to return.</typeparam>
    /// <param name="value">The value to align.</param>
    /// <param name="size">The alignment size.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AlignUp<T>(this T value, T size) where T : ISubtractionOperators<T, T, T>, IModulusOperators<T, T, T>
    {
        return (size - value % size) % size;
    }

    /// <summary>
    /// Align <paramref name="value"/> down to <paramref name="size"/> and return the result.
    /// </summary>
    /// <typeparam name="T">The integral type to return.</typeparam>
    /// <param name="value">The value to align.</param>
    /// <param name="size">The alignment size.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AlignDown<T>(this T value, T size) where T : IUnaryNegationOperators<T, T>, IModulusOperators<T, T, T>
    {
        return -(value % size);
    }
}
