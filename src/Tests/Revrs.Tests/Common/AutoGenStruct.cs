using System.Runtime.InteropServices;
using Revrs.Attributes;

namespace Revrs.Tests.Common;

[Reversable]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public partial struct AutoGenStruct
{
    [DoNotReverse]
    public uint Magic_Skipped;
    
    public int Int32;
    
    public short Int16_ExpectPadding;
    
    public NestedAutoGenStruct Nested;
    
    public PrimitiveStruct Primitive;
    
    public byte Byte;
    
    public unsafe fixed int Fixed[4];
}

[Reversable]
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public partial struct NestedAutoGenStruct
{
    public int Int32;
    
    public short Int16_NoPadding;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct PrimitiveStruct
{
    public int Int32;
    
    public long Int64;
    
    public ushort Int16;
    
    public long Int64_2;
    
    public PrimitiveStruct2 SubStruct;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public struct PrimitiveStruct2
{
    public ushort Int16;
    
    public long Int64;
}