namespace Revrs;

/// <summary>
/// Extension methods for reading types directly from memory.
/// </summary>
public static unsafe class MemUtils
{
    /// <summary>
    /// Read <typeparamref name="T"/> from memory relative to a given <typeparamref name="TOwner"/> 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <typeparam name="TOwner"></typeparam>
    /// <returns></returns>
    public static ref T GetRelativeTo<T, TOwner>(in TOwner owner, int offset)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TOwner : unmanaged
    {
        fixed (TOwner* loc = &owner) {
            byte* ptr = (byte*)loc + offset;
            return ref *(T*)ptr;
        }
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from memory relative to a given pointer 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <returns></returns>
    public static ref T GetRelativeTo<T>(void* owner, int offset)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        byte* ptr = (byte*)owner + offset;
        return ref *(T*)ptr;
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from memory relative to a given <typeparamref name="TOwner"/> 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <typeparam name="TOwner"></typeparam>
    public static ref T GetRelativeTo<T, TOwner>(in TOwner owner, uint offset)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TOwner : unmanaged
    {
        fixed (TOwner* loc = &owner) {
            byte* ptr = (byte*)loc + offset;
            return ref *(T*)ptr;
        }
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from memory relative to a given pointer 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <returns></returns>
    public static ref T GetRelativeTo<T>(void* owner, uint offset)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        byte* ptr = (byte*)owner + offset;
        return ref *(T*)ptr;
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from memory relative to a given <typeparamref name="TOwner"/> 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <param name="count">The number of objects to read from memory</param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <typeparam name="TOwner"></typeparam>
    public static Span<T> GetSpanRelativeTo<T, TOwner>(this ref TOwner owner, int offset, int count)
        where T : unmanaged
        where TOwner : unmanaged
    {
        fixed (TOwner* loc = &owner) {
            byte* ptr = (byte*)loc + offset;
            return new Span<T>(ptr, count);
        }
    }

    /// <summary>
    /// Read a span of <typeparamref name="T"/> from memory relative to a given pointer 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <param name="count">The number of objects to read from memory</param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    public static Span<T> GetSpanRelativeTo<T>(void* owner, int offset, int count)
        where T : unmanaged
    {
        byte* ptr = (byte*)owner + offset;
        return new Span<T>(ptr, count);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from memory relative to a given <typeparamref name="TOwner"/> 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <param name="count">The number of objects to read from memory</param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <typeparam name="TOwner"></typeparam>
    public static Span<T> GetSpanRelativeTo<T, TOwner>(in TOwner owner, uint offset, int count)
        where T : unmanaged
        where TOwner : unmanaged
    {
        fixed (TOwner* loc = &owner) {
            byte* ptr = (byte*)loc + offset;
            return new Span<T>(ptr, count);
        }
    }

    /// <summary>
    /// Read a span of <typeparamref name="T"/> from memory relative to a given pointer 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <param name="count">The number of objects to read from memory</param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    public static Span<T> GetSpanRelativeTo<T>(void* owner, uint offset, int count)
        where T : unmanaged
    {
        byte* ptr = (byte*)owner + offset;
        return new Span<T>(ptr, count);
    }
}