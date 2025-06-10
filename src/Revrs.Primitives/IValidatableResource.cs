namespace Revrs.Primitives;

/// <summary>
/// Defines functions for validating an instance of <typeparamref name="T"/> cast from unknown memory 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IValidatableResource<T> where T : unmanaged
{
    /// <summary>
    /// Validate the input <paramref name="resource"/> and return the first occuring error.
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    public static abstract Exception? Validate(in T resource);
}