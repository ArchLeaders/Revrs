using System.Runtime.CompilerServices;

namespace Revrs.Primitives;

public readonly unsafe struct Result<T> where T : unmanaged
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    public static readonly Result<T> Empty = default;
    
    private readonly Exception? _exception;
    private readonly T* _value;

    public Result(ref T value)
    {
        _value = (T*)Unsafe.AsPointer(ref value);
    }

    public Result(ref T value, Exception? exception)
    {
        _value = (T*)Unsafe.AsPointer(ref value);
        _exception = exception;
    }

    public Result(Exception exception)
    {
        _value = null;
        _exception = exception;
    }

    public ref T Value {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref *_value;
    }

    public ref T TryGet(out Exception? exception)
    {
        if ((exception = _exception) is not null) {
            return ref Unsafe.NullRef<T>();
        }

        return ref *_value;
    }

    public ref T GetOrThrow()
    {
        if (_exception is not null) {
            throw _exception;
        }

        return ref *_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Err(Exception exception) => new(exception);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok(ref T value) => new(ref value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T>(T* value) => new(ref Unsafe.AsRef<T>(value));
}