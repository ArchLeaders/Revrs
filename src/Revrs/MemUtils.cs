using System.Diagnostics.Contracts;

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
    [Pure]
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
    [Pure]
    public static ref T GetRelativeTo<T>(void* owner, int offset)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        return ref *(T*)((IntPtr)owner + offset);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from memory relative to a given <typeparamref name="TOwner"/> 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <typeparam name="TOwner"></typeparam>
    [Pure]
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
    [Pure]
    public static ref T GetRelativeTo<T>(void* owner, uint offset)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        return ref *(T*)((IntPtr)owner + offset);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from memory relative to a given <typeparamref name="TOwner"/> 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <param name="count">The number of objects to read from memory</param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <typeparam name="TOwner"></typeparam>
    [Pure]
    public static Span<T> GetSpanRelativeTo<T, TOwner>(in TOwner owner, int offset, int count)
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
    [Pure]
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
    [Pure]
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
    [Pure]
    public static Span<T> GetSpanRelativeTo<T>(void* owner, uint offset, int count)
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
    /// <param name="isEnd">Predicate to determine when to stop reading</param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <typeparam name="TOwner"></typeparam>
    [Pure]
    public static Span<T> GetSpanRelativeTo<T, TOwner>(in TOwner owner, int offset, Func<byte, bool> isEnd)
        where T : unmanaged
        where TOwner : unmanaged
    {
        fixed (TOwner* loc = &owner) {
            byte* ptr = (byte*)loc + offset;
            
            int count = -1;
            while (!isEnd(ptr[++count])) { }
            
            return new Span<T>(ptr, count);
        }
    }

    /// <summary>
    /// Read a span of <typeparamref name="T"/> from memory relative to a given pointer 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <param name="isEnd">Predicate to determine when to stop reading</param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    [Pure]
    public static Span<T> GetSpanRelativeTo<T>(void* owner, int offset, Func<byte, bool> isEnd)
        where T : unmanaged
    {
        byte* ptr = (byte*)owner + offset;
        
        int count = -1;
        while (!isEnd(ptr[++count])) { }
        
        return new Span<T>(ptr, count);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from memory relative to a given <typeparamref name="TOwner"/> 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <param name="isEnd">Predicate to determine when to stop reading</param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    /// <typeparam name="TOwner"></typeparam>
    [Pure]
    public static Span<T> GetSpanRelativeTo<T, TOwner>(in TOwner owner, uint offset, Func<byte, bool> isEnd)
        where T : unmanaged
        where TOwner : unmanaged
    {
        fixed (TOwner* loc = &owner) {
            byte* ptr = (byte*)loc + offset;
            
            int count = -1;
            while (!isEnd(ptr[++count])) { }
            
            return new Span<T>(ptr, count);
        }
    }

    /// <summary>
    /// Read a span of <typeparamref name="T"/> from memory relative to a given pointer 
    /// </summary>
    /// <param name="owner">The starting point in memory to look for <typeparamref name="T"/></param>
    /// <param name="offset">The offset from the owner to look for <typeparamref name="T"/></param>
    /// <param name="isEnd">Predicate to determine when to stop reading</param>
    /// <typeparam name="T">The type to read from memory</typeparam>
    [Pure]
    public static Span<T> GetSpanRelativeTo<T>(void* owner, uint offset, Func<byte, bool> isEnd)
        where T : unmanaged
    {
        byte* ptr = (byte*)owner + offset;

        int count = -1;
        while (!isEnd(ptr[++count])) { }
        
        return new Span<T>(ptr, count + 1);
    }

    /// <summary>
    /// Read a string of bytes from memory relative to a given <typeparamref name="TOwner"/> 
    /// </summary>
    /// <param name="owner">The starting point in memory to start reading the string</param>
    /// <param name="offset">The offset to start reading the string</param>
    /// <typeparam name="TOwner"></typeparam>
    [Pure]
    public static Span<byte> GetStrRelativeTo<TOwner>(in TOwner owner, int offset)
        where TOwner : unmanaged
    {
        fixed (TOwner* loc = &owner) {
            byte* ptr = (byte*)loc + offset;
            
            int count = -1;
            while (ptr[++count] is not 0) { }
            
            return new Span<byte>(ptr, count);
        }
    }

    /// <summary>
    /// Read a string of bytes from memory relative to a given pointer
    /// </summary>
    /// <param name="owner">The starting point in memory to start reading the string</param>
    /// <param name="offset">The offset to start reading the string</param>
    [Pure]
    public static Span<byte> GetStrRelativeTo(void* owner, int offset)
    {
        byte* ptr = (byte*)owner + offset;
        
        int count = -1;
        while (ptr[++count] is not 0) { }
        
        return new Span<byte>(ptr, count);
    }

    /// <summary>
    /// Read a string of bytes from memory relative to a given <typeparamref name="TOwner"/> 
    /// </summary>
    /// <param name="owner">The starting point in memory to start reading the string</param>
    /// <param name="offset">The offset to start reading the string</param>
    /// <typeparam name="TOwner"></typeparam>
    [Pure]
    public static Span<byte> GetStrRelativeTo<TOwner>(in TOwner owner, uint offset)
        where TOwner : unmanaged
    {
        fixed (TOwner* loc = &owner) {
            byte* ptr = (byte*)loc + offset;
            
            int count = -1;
            while (ptr[++count] is not 0) { }
            
            return new Span<byte>(ptr, count);
        }
    }

    /// <summary>
    /// Read a string of bytes from memory relative to a given pointer
    /// </summary>
    /// <param name="owner">The starting point in memory to start reading the string</param>
    /// <param name="offset">The offset to start reading the string</param>
    [Pure]
    public static Span<byte> GetStrRelativeTo(void* owner, uint offset)
    {
        byte* ptr = (byte*)owner + offset;

        int count = -1;
        while (ptr[++count] is not 0) { }
        
        return new Span<byte>(ptr, count + 1);
    }
}