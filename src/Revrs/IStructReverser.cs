namespace Revrs;

/// <summary>
/// Defines a static function to reverse a <see cref="Span{T}"/> in predefined slices.
/// <para>
/// For optimal performance, <see cref="IStructReverser"/> should be implemented by a dedicated <see langword="class"/> inside the target <see langword="struct"/>, not implemented by the <see langword="struct"/> itself.
/// </para>
/// </summary>
public interface IStructReverser
{
    /// <summary>
    /// Reverses the <paramref name="buffer"/> in predefined slices to match the target <see langword="struct"/>.
    /// </summary>
    /// <param name="buffer"></param>
    public static abstract void Reverse(in Span<byte> buffer);
}
