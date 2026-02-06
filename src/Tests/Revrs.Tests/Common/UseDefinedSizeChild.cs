using System.Runtime.InteropServices;
using Revrs.Attributes;

namespace Revrs.Tests.Common;

[Reversible]
[StructLayout(LayoutKind.Explicit, Size = 4, Pack = 4)]
public partial struct UseDefinedSizeChild
{
    [field: FieldOffset(0)]
    public int A { get; set; }
    
    [field: FieldOffset(0), DoNotReverse]
    public uint B { get; set; }
}