namespace Revrs.Attributes;

/// <summary>
/// Annotate types with this attribute to automatically generate IReversable.Reverse
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
public sealed class ReversableAttribute : Attribute;