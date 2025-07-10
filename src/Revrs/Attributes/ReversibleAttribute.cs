namespace Revrs.Attributes;

/// <summary>
/// Annotate types with this attribute to automatically generate IStructReverser.Reverse
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
public sealed class ReversibleAttribute : Attribute;