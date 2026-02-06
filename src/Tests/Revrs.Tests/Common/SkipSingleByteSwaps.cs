using Revrs.Attributes;

namespace Revrs.Tests.Common;

[Reversible]
public partial struct SkipSingleByteSwaps
{
    public byte A { get; set; }
    public byte B { get; set; }
    public byte C { get; set; }
    public byte D { get; set; }
    public int Swap { get; set; }
}