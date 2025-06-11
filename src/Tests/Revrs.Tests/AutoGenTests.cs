using Revrs.Extensions;
using Revrs.Tests.Common;

namespace Revrs.Tests;

public unsafe class AutoGenTests
{
    private static readonly byte[] _dataBe
        = "\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F\x20\x21\x22\x23\x24\x25\x26\x27\x28\x28\x29\x2A\x2B\x2C\x2D\x2E\x2F\x40\x00\x00\x00\x30\x31\x32\x33\x34\x35\x36\x37\x38\x39\x3A\x3B\x3C\x3D\x3E\x3F"u8.ToArray();
    
    private static readonly byte[] _dataLe
        = "\x01\x02\x03\x04\x08\x07\x06\x05\x0A\x09\x0E\x0D\x0C\x0B\x10\x0F\x14\x13\x12\x11\x1C\x1B\x1A\x19\x18\x17\x16\x15\x1E\x1D\x26\x25\x24\x23\x22\x21\x20\x1F\x28\x27\x2F\x2E\x2D\x2C\x2B\x2A\x29\x28\x40\x00\x00\x00\x33\x32\x31\x30\x37\x36\x35\x34\x3B\x3A\x39\x38\x3F\x3E\x3D\x3C"u8.ToArray();

    private static readonly AutoGenStruct _struct;

    static AutoGenTests()
    {
        _struct = new AutoGenStruct {
            Magic_Skipped = 0x04030201,
            Int32 = 0x05060708,
            Int16_ExpectPadding = 0x090A,
            Nested = new NestedAutoGenStruct {
                Int32 = 0x0B0C0D0E,
                Int16_NoPadding = 0x0F10,
            },
            Primitive = new PrimitiveStruct {
                Int32 = 0x11121314,
                Int64 = 0x15161718191A1B1C,
                Int16 = 0x1D1E,
                Int64_2 = 0x1F20212223242526,
                SubStruct = new PrimitiveStruct2 {
                    Int16 = 0x2728,
                    Int64 = 0x28292A2B2C2D2E2F,
                }
            },
            Byte = 0x40
        };
        
        _struct.Fixed[0] = 0x30313233;
        _struct.Fixed[1] = 0x34353637;
        _struct.Fixed[2] = 0x38393A3B;
        _struct.Fixed[3] = 0x3C3D3E3F;
    }
    
    [Fact]
    public void VerifyReadBe()
    {
        ref AutoGenStruct res = ref _dataBe.AsSpan().ReadStruct<AutoGenStruct>(Endianness.Big);
        res.Should().BeEquivalentTo(_struct);
    }
    
    [Fact]
    public void VerifyReadLe()
    {
        ref AutoGenStruct res = ref _dataLe.AsSpan().ReadStruct<AutoGenStruct>(Endianness.Little);
        res.Should().BeEquivalentTo(_struct);
    }
}