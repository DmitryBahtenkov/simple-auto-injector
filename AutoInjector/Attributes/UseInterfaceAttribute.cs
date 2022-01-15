namespace AutoInjector.Attributes;

/// <summary>
/// Set a concrete interface to resolve dependency
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class UseInterfaceAttribute : Attribute
{
    /// <summary>
    /// Name of interface for resolve dependency
    /// </summary>
    public string InterfaceName { get; }

    /// <summary>
    /// Create UseInterface attribute
    /// </summary>
    /// <param name="interfaceName"></param>
    public UseInterfaceAttribute(string interfaceName)
    {
        InterfaceName = interfaceName;
    }
}
