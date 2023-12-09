using Revrs.Extensions;
using Revrs.Tests.Common;

namespace Revrs.Tests.ExtensionsTests.WriterExtensionsTests;

public class WriterExtensionsOperationsTests
{
    [Fact]
    public void WritePrimitive_ShouldBeSystemByteOrder()
    {
        using MemoryStream stream = new();
        stream.Write(uint.MaxValue / 2);
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read<uint>().Should().Be(uint.MaxValue / 2);
    }

    [Fact]
    public void WriteStruct_ShouldBeSystemByteOrder()
    {
        GenericStructure genericStructure = new();

        using MemoryStream stream = new();
        stream.Write(genericStructure);
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read<GenericStructure>().Should().Be(genericStructure);
    }

    [Fact]
    public void WritePrimitive_ShouldBeEndianNeutral()
    {
        using MemoryStream streamA = new();
        streamA.Write(uint.MaxValue / 2, Endianness.Little);
        streamA.Seek(0, SeekOrigin.Begin);
        streamA.Read<uint>(Endianness.Little).Should().Be(uint.MaxValue / 2);

        using MemoryStream streamB = new();
        streamB.Write(uint.MaxValue / 2, Endianness.Big);
        streamB.Seek(0, SeekOrigin.Begin);
        streamB.Read<uint>(Endianness.Big).Should().Be(uint.MaxValue / 2);
    }

    [Fact]
    public void WriteStruct_ShouldBeEndianNeutral()
    {
        GenericStructure genericStructureA = new();

        using MemoryStream streamA = new();
        streamA.Write<GenericStructure, GenericStructure.Reverser>(genericStructureA, Endianness.Big);
        streamA.Seek(0, SeekOrigin.Begin);
        streamA.Read<GenericStructure, GenericStructure.Reverser>(Endianness.Big).Should().Be(genericStructureA);

        GenericStructure genericStructureB = new();

        using MemoryStream streamB = new();
        streamB.Write<GenericStructure, GenericStructure.Reverser>(genericStructureB, Endianness.Big);
        streamB.Seek(0, SeekOrigin.Begin);
        streamB.Read<GenericStructure, GenericStructure.Reverser>(Endianness.Big).Should().Be(genericStructureB);
    }
}
