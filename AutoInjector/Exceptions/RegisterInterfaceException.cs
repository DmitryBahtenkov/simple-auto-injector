using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SimpleAutoInjector.Tests")]
namespace AutoInjector.Exceptions;

internal sealed class RegisterInterfaceException : Exception
{
    public RegisterInterfaceException(string interfaceName) 
            : base($"Unable to register service with interface {interfaceName}") {}
}
