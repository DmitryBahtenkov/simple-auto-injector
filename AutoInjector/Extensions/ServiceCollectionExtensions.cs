using Microsoft.Extensions.DependencyInjection;

namespace AutoInjector.Extensions;

public static class ServiceCollectionExtensions
{
    internal static Action<IocOptions> OptionsDelegate { get; private set; }

    /// <summary>
    /// Add or replace dependency in container
    /// </summary>
    /// <param name="serviceCollection">di-container</param>
    /// <param name="lifetime">lifetime for new service</param>
    /// <typeparam name="TInterface">interface type for service</typeparam>
    /// <typeparam name="TImplementation">implementation type for service</typeparam>
    /// <returns></returns>
    public static void AddOrUpdate<TInterface, TImplementation>(this IServiceCollection serviceCollection, ServiceLifetime lifetime) 
        where TImplementation : class, TInterface where TInterface : class
    {
        var existDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(TInterface));
            
        // if exist, remove it 
        if (existDescriptor is not null)
        {
            serviceCollection.Remove(existDescriptor);
        }
            
        serviceCollection.Add(new ServiceDescriptor(typeof(TInterface), typeof(TImplementation), lifetime));
    }

    /// <summary>
    /// Add autoInjector options to DI
    /// </summary>
    /// <param name="serviceCollection">di-container</param>
    /// <param name="options">delegate for set options</param>
    public static void AddAutoInjector(this IServiceCollection serviceCollection, Action<IocOptions> options)
    {
        OptionsDelegate += options;
    }        
}
