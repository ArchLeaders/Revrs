using Revrs.Attributes;

namespace Revrs.Tests.Common;

[Reversible]
public partial struct UseDefinedSizeParent
{
    public UseDefinedSizeChild _1 { get; set; }
    public UseDefinedSizeChild _2 { get; set; }
}