using System.Runtime.InteropServices;

namespace Revrs;

/// <summary>
/// Utility class for reversing data types
/// </summary>
public class RevrsUtils
{
    /// <summary>
    /// Byte swap a <see cref="ushort"/> and return the result
    /// </summary>
    /// <param name="u16"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Swap(ushort u16)
    {
        return (ushort)((ushort)((u16 & 0xff) << 8) | ((u16 >> 8) & 0xff));
    }

    /// <summary>
    /// Inline byte swap a <see cref="ushort"/>
    /// </summary>
    /// <param name="u16"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref ushort u16) => u16 = Swap(u16);

    /// <inheritdoc cref="Swap(ref ushort)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap(ushort* u16) => *u16 = Swap(*u16);
    
    /// <summary>
    /// Byte swap a <see cref="short"/> and return the result
    /// </summary>
    /// <param name="s16"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Swap(short s16)
    {
        return (short)((ushort)((s16 & 0xff) << 8) | ((s16 >> 8) & 0xff));
    }

    /// <summary>
    /// Inline byte swap a <see cref="short"/>
    /// </summary>
    /// <param name="s16"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref short s16) => s16 = Swap(s16);

    /// <inheritdoc cref="Swap(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap(short* s16) => *s16 = Swap(*s16);

    /// <summary>
    /// Byte swap a <see cref="uint"/> and return the result
    /// </summary>
    /// <param name="u32"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Swap(uint u32)
    {
        u32 = (u32 >> 16) | (u32 << 16);
        return ((u32 & 0xFF00FF00) >> 8) | ((u32 & 0x00FF00FF) << 8);
    }

    /// <summary>
    /// Inline byte swap a <see cref="uint"/>
    /// </summary>
    /// <param name="u32"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref uint u32) => u32 = Swap(u32);

    /// <inheritdoc cref="Swap(ref uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap(uint* u32) => *u32 = Swap(*u32);

    /// <summary>
    /// Byte swap a <see cref="int"/> and return the result
    /// </summary>
    /// <param name="s32"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Swap(int s32)
    {
        s32 = (s32 >> 16) | (s32 << 16);
        return (int)((s32 & 0xFF00FF00) >> 8) | ((s32 & 0x00FF00FF) << 8);
    }

    /// <summary>
    /// Inline byte swap a <see cref="int"/>
    /// </summary>
    /// <param name="s32"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref int s32) => s32 = Swap(s32);

    /// <inheritdoc cref="Swap(ref int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap(int* s32) => *s32 = Swap(*s32);

    /// <summary>
    /// Byte swap a <see cref="float"/> and return the result
    /// </summary>
    /// <param name="f32"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe float Swap(float f32)
    {
        _32bitUnion union = new(f32);
        Swap(union.GetPtr());
        return union.F32;
    }

    /// <summary>
    /// Inline byte swap a <see cref="float"/>
    /// </summary>
    /// <param name="f32"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref float f32) => f32 = Swap(f32);

    /// <inheritdoc cref="Swap(ref float)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap(float* f32) => Swap((uint*)f32);

    /// <summary>
    /// Byte swap a <see cref="ulong"/> and return the result
    /// </summary>
    /// <param name="s64"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Swap(ulong s64)
    {
        s64 = (s64 >> 32) | (s64 << 32);
        s64 = ((s64 & 0xFFFF0000FFFF0000) >> 16) | ((s64 & 0x0000FFFF0000FFFF) << 16);
        return ((s64 & 0xFF00FF00FF00FF00) >> 8) | ((s64 & 0x00FF00FF00FF00FF) << 8);
    }

    /// <summary>
    /// Inline byte swap a <see cref="ulong"/>
    /// </summary>
    /// <param name="s64"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref ulong s64) => s64 = Swap(s64);

    /// <inheritdoc cref="Swap(ref ulong)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap(ulong* u64) => *u64 = Swap(*u64);

    /// <summary>
    /// Byte swap a <see cref="long"/> and return the result
    /// </summary>
    /// <param name="s64"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Swap(long s64)
    {
        unchecked {
            return (long)Swap((ulong)s64);
        }
    }

    /// <summary>
    /// Inline byte swap a <see cref="long"/>
    /// </summary>
    /// <param name="s64"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref long s64) => s64 = Swap(s64);

    /// <inheritdoc cref="Swap(ref long)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap(long* s64) => *s64 = Swap(*s64);

    /// <summary>
    /// Byte swap a <see cref="double"/> and return the result
    /// </summary>
    /// <param name="f64"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe double Swap(double f64)
    {
        _64bitUnion union = new(f64);
        Swap(union.GetPtr());
        return union.F64;
    }

    /// <summary>
    /// Inline byte swap a <see cref="double"/>
    /// </summary>
    /// <param name="f64"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap(ref double f64) => f64 = Swap(f64);

    /// <inheritdoc cref="Swap(ref double)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap(double* f64) => Swap((ulong*)f64);
}

[StructLayout(LayoutKind.Explicit)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
file struct _32bitUnion(float f32)
{
    [FieldOffset(0)]
    public uint U32;
    
    [FieldOffset(0)]
    public float F32 = f32;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe uint* GetPtr()
    {
        fixed (uint* ptr = &U32) {
            return ptr;
        }
    }
}

[StructLayout(LayoutKind.Explicit)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
file struct _64bitUnion(double f64)
{
    [FieldOffset(0)]
    public ulong U64;
    
    [FieldOffset(0)]
    public double F64 = f64;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ulong* GetPtr()
    {
        fixed (ulong* ptr = &U64) {
            return ptr;
        }
    }
}