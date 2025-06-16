using System.Runtime.InteropServices;

namespace Revrs.Primitives;

public unsafe class Resource<T>(ArraySegment<byte> buffer) where T : unmanaged, IValidatableResource<T>
{
    /// <summary>
    /// The constant size of the unmanaged type
    /// </summary>
    private static readonly int _resourceSize = sizeof(T);

    /// <summary>
    /// The underlying byte-memory of the interpreted resource
    /// </summary>
    private readonly Memory<byte> _memory = buffer.AsMemory(0, _resourceSize);

    /// <summary>
    /// Cast the resource memory into <typeparamref name="T"/> and return the validated <see cref="Result{T}"/>
    /// </summary>
    public Result<T> Value {
        get {
            ref T result = ref MemoryMarshal.Cast<byte, T>(_memory.Span)[0];
            return new Result<T>(ref result, T.Validate(ref result));
        }
    }

    /// <summary>
    /// Cast the resource memory into <typeparamref name="T"/>, catching any unhandled exceptions, and return the validated <see cref="Result{T}"/>
    /// </summary>
    public Result<T> GetSafe()
    {
        try {
            ref T result = ref MemoryMarshal.Cast<byte, T>(_memory.Span)[0];
            return new Result<T>(ref result, T.Validate(ref result));
        }
        catch (Exception ex) {
            return Result<T>.Err(ex);
        }
    }
}