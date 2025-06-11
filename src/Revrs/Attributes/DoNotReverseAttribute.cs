namespace Revrs.Attributes;

/// <summary>
/// This field will not be reversed with the rest of the struct.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class DoNotReverseAttribute : Attribute;