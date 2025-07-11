// ReSharper disable InconsistentNaming

using BenchmarkDotNet.Attributes;

namespace Revrs.Benchmarks;

public class SwapBenchmark
{
    private readonly byte[] _data = "ABCDEFGH"u8.ToArray();
    private readonly ulong _u64 = 0x4847464544434241;
    private readonly long _s64 = 0x4847464544434241;
    private readonly double _f64 = 0x4847464544434241;
    
    private readonly uint _u32_1 = 0x5DA0009;
    private readonly int _s32_1 = 0x5DA0009;
    private readonly float _f32_1 = 0x5DA0009;
    
    private readonly uint _u32_2 = 0x4AB8702;
    private readonly int _s32_2 = 0x4AB8702;
    private readonly float _f32_2 = 0x4AB8702;

    [Benchmark]
    public void Reverse()
    {
        _data.AsSpan().Reverse();
    }
    
    [Benchmark]
    public unsafe void Swap()
    {
        fixed (ulong* ptr = &_u64) {
            RevrsUtils.Swap(ptr);        
        }
    }
    
    [Benchmark]
    public unsafe void SwapCastSigned()
    {
        fixed (long* ptr = &_s64) {
            RevrsUtils.Swap(ptr);
        }
    }
    
    [Benchmark]
    public unsafe void SwapCastFloat()
    {
        fixed (double* ptr = &_f64) {
            RevrsUtils.Swap(ptr);        
        }
    }

    [Benchmark]
    public void Reverse2()
    {
        Span<byte> data = _data.AsSpan();
        data[..4].Reverse();
        data[4..8].Reverse();
    }
    
    [Benchmark]
    public unsafe void Swap2()
    {
        fixed (uint* ptr = &_u32_1) {
            RevrsUtils.Swap(ptr);        
        }
        
        fixed (uint* ptr = &_u32_2) {
            RevrsUtils.Swap(ptr);        
        }
    }
    
    [Benchmark]
    public unsafe void Swap2CastSigned()
    {
        fixed (int* ptr = &_s32_1) {
            RevrsUtils.Swap(ptr);        
        }
        
        fixed (int* ptr = &_s32_2) {
            RevrsUtils.Swap(ptr);        
        }
    }
    
    [Benchmark]
    public unsafe void Swap2CastFloat()
    {
        fixed (float* ptr = &_f32_1) {
            RevrsUtils.Swap(ptr);        
        }
        
        fixed (float* ptr = &_f32_2) {
            RevrsUtils.Swap(ptr);        
        }
    }
}