using Revrs.Extensions;
using Revrs.Tests.Common;

namespace Revrs.Tests.ExtensionsTests.ReaderExtensionsTests;

public class ReaderExtensionsOperationsTests
{
    [Fact]
    public void ReadPrimitive_ShouldBeSystemByteOrder()
    {
        Span<byte> input = [ 0x01, 0x00 ];
        if (BitConverter.IsLittleEndian) {
            input.Read<ushort>().Should().Be(0x0001);
        }
        else {
            input.Read<ushort>().Should().Be(0x0100);
        }
    }

    [Fact]
    public void ReadStruct_ShouldBeSystemByteOrder()
    {
        Span<byte> input = [
            0xFE, 0xFF,
            0xFF, 0x00,
            0xFF, 0x7F,
            0xFF, 0xFF, 0xFF, 0x7F,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F
        ];

        GenericStructure @struct = input.Read<GenericStructure>();

        if (BitConverter.IsLittleEndian) {
            @struct.Endianness.Should().Be(Endianness.Little);
            @struct.UInt8.Should().Be(0xFF);
            @struct.UInt16.Should().Be(0x7F_FF);
            @struct.UInt32.Should().Be(0x7F_FF_FF_FF);
            @struct.UInt64.Should().Be(0x7F_FF_FF_FF_FF_FF_FF_FF);
        }
        else {
            @struct.Endianness.Should().Be(Endianness.Big);
            @struct.UInt8.Should().Be(0xFF);
            @struct.UInt16.Should().Be(0xFF7F);
            @struct.UInt32.Should().Be(0xFFFF_FF7F);
            @struct.UInt64.Should().Be(0xFFFF_FFFF_FFFF_FF7F);
        }
    }

    [Fact]
    public void ReadSpanWithCount_ShouldTruncateResullt()
    {
        Span<byte> input = [
            0xFF, 0x7F,
            0x7F, 0xFF,
            0xFF, 0x7F,
            0x7F, 0xFF,
        ];

        input.ReadSpan<ushort>(3).Length.Should().Be(3);
    }

    [Fact]
    public void ReadSpan_ShouldNotTruncateResult()
    {
        Span<byte> input = [
            0xFF, 0x7F,
            0x7F, 0xFF,
            0xFF, 0x7F,
            0x7F, 0xFF,
        ];

        input.ReadSpan<ushort>().Length.Should().Be(4);
    }

    [Fact]
    public void ReadPrimitive_ShouldBeEndianNeutral()
    {
        Span<byte> inputA = [ 0x01, 0x00 ];
        inputA.Read<ushort>(Endianness.Little).Should().Be(1);

        Span<byte> inputB = [ 0x00, 0x01 ];
        inputB.Read<ushort>(Endianness.Big).Should().Be(1);
    }

    [Fact]
    public void ReadStruct_ShouldBeEndianNeutral()
    {
        Span<byte> inputA = [
            0xFE, 0xFF,
            0xFF, 0x00,
            0xFF, 0x7F,
            0xFF, 0xFF, 0xFF, 0x7F,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F
        ];

        GenericStructure structA = inputA.Read
            <GenericStructure, GenericStructure.Reverser>(Endianness.Little);
        structA.Endianness.Should().Be(Endianness.Little);
        structA.UInt8.Should().Be(byte.MaxValue);
        structA.UInt16.Should().Be(ushort.MaxValue / 2);
        structA.UInt32.Should().Be(uint.MaxValue / 2);
        structA.UInt64.Should().Be(ulong.MaxValue / 2);

        Span<byte> inputB = [
            0xFF, 0xFE,
            0xFF, 0x00,
            0x7F, 0xFF,
            0x7F, 0xFF, 0xFF, 0xFF,
            0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
        ];

        GenericStructure structB = inputB.Read
            <GenericStructure, GenericStructure.Reverser>(Endianness.Big);
        structB.Endianness.Should().Be(Endianness.Big);
        structB.UInt8.Should().Be(byte.MaxValue);
        structB.UInt16.Should().Be(ushort.MaxValue / 2);
        structB.UInt32.Should().Be(uint.MaxValue / 2);
        structB.UInt64.Should().Be(ulong.MaxValue / 2);
    }

    [Fact]
    public void ReadPrimitiveSpan_ShouldBeEndianNeutral()
    {
        Span<byte> inputA = [
            0xFF, 0x7F,
            0x7F, 0xFF,
            0xFF, 0x7F,
            0x7F, 0xFF,
        ];

        Span<ushort> spanA = inputA.ReadSpan<ushort>(4, Endianness.Little);
        spanA[0].Should().Be(0x7FFF);
        spanA[1].Should().Be(0xFF7F);
        spanA[2].Should().Be(0x7FFF);
        spanA[3].Should().Be(0xFF7F);

        Span<byte> inputB = [
            0x7F, 0xFF,
            0xFF, 0x7F,
            0x7F, 0xFF,
            0xFF, 0x7F,
        ];

        Span<ushort> spanB = inputB.ReadSpan<ushort>(4, Endianness.Big);
        spanB[0].Should().Be(0x7FFF);
        spanB[1].Should().Be(0xFF7F);
        spanB[2].Should().Be(0x7FFF);
        spanB[3].Should().Be(0xFF7F);
    }

    [Fact]
    public void ReadStructSpan_ShouldBeEndianNeutral()
    {
        Span<byte> inputA = [
            0xFE, 0xFF,
            0xFF, 0x00,
            0xFF, 0x7F,
            0xFF, 0xFF, 0xFF, 0x7F,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F,

            0xFE, 0xFF,
            0xFF, 0x00,
            0xFF, 0x7F,
            0xFF, 0xFF, 0xFF, 0x7F,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F
        ];

        Span<GenericStructure> spanA = inputA.ReadSpan
            <GenericStructure, GenericStructure.Reverser>(2, Endianness.Little);

        for (int i = 0; i < 2; i++) {
            spanA[i].Endianness.Should().Be(Endianness.Little);
            spanA[i].UInt8.Should().Be(byte.MaxValue);
            spanA[i].UInt16.Should().Be(ushort.MaxValue / 2);
            spanA[i].UInt32.Should().Be(uint.MaxValue / 2);
            spanA[i].UInt64.Should().Be(ulong.MaxValue / 2);
        }

        Span<byte> inputB = [
            0xFF, 0xFE,
            0xFF, 0x00,
            0x7F, 0xFF,
            0x7F, 0xFF, 0xFF, 0xFF,
            0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,

            0xFF, 0xFE,
            0xFF, 0x00,
            0x7F, 0xFF,
            0x7F, 0xFF, 0xFF, 0xFF,
            0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
        ];

        Span<GenericStructure> spanB = inputB.ReadSpan
            <GenericStructure, GenericStructure.Reverser>(2, Endianness.Big);

        for (int i = 0; i < 2; i++) {
            spanB[i].Endianness.Should().Be(Endianness.Big);
            spanB[i].UInt8.Should().Be(byte.MaxValue);
            spanB[i].UInt16.Should().Be(ushort.MaxValue / 2);
            spanB[i].UInt32.Should().Be(uint.MaxValue / 2);
            spanB[i].UInt64.Should().Be(ulong.MaxValue / 2);
        }
    }
}
