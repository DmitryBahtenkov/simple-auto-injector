using System.Reflection;
using AutoInjector.Attributes;
using AutoInjector.Exceptions;
using AutoInjector.LifeTime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoInjector
{
    public class DependencyBuilder
    {
        private readonly string[] _interfaceNames = {nameof(IScoped), nameof(ITransient), nameof(ISingleton)};

        public IConfiguration Configuration { get; }

        private readonly IocOptions _opt;
        public IocOptions Options 
        { 
            get 
            {
                return !string.IsNullOrEmpty(_opt.ProjectKey) ? _opt : throw new ArgumentNullException(nameof(IocOptions.ProjectKey));
            } 
        }

        public static DependencyBuilder CreateBuilder(IConfiguration configuration, Action<IocOptions> options)
        {
            return new(configuration, options);
        }

        internal DependencyBuilder(IConfiguration configuration, Action<IocOptions> options)
        {
            Configuration = configuration;
            var opt = new IocOptions();
            options.Invoke(opt);

            _opt = opt;
        }

        public void Build(IServiceCollection serviceCollection)
        {
            var scoped = typeof(IScoped);
            var singleton = typeof(ISingleton);
            var transient = typeof(ITransient);

            var allTypes = GetAllTypes();
            
            var scopedTypes = allTypes.Where(x => x.IsClass && scoped.IsAssignableFrom(x)).ToArray();
            var singletonTypes = allTypes.Where(x => x.IsClass && singleton.IsAssignableFrom(x)).ToArray();
            var transientTypes = allTypes.Where(x => x.IsClass && transient.IsAssignableFrom(x)).ToArray();

            FillScoped(scopedTypes, serviceCollection);
            FillTransient(transientTypes, serviceCollection);
            FillSingleton(singletonTypes, serviceCollection);
        }
        
        private void FillScoped(IEnumerable<Type> scopedTypes, IServiceCollection serviceCollection)
        {
            foreach(var scopedType in scopedTypes)
            {
                var @interface = GetInterface(scopedType);

                if (@interface is not null)
                {
                    serviceCollection.AddScoped(@interface, scopedType);
                }
                else
                {
                    serviceCollection.AddScoped(scopedType);
                }
            };
        }

        private void FillTransient(IEnumerable<Type> transientTypes, IServiceCollection serviceCollection)
        {
            foreach(var transientType in transientTypes)
            {
                var @interface = GetInterface(transientType);
                if (@interface is not null)
                {
                    serviceCollection.AddTransient(@interface, transientType);
                }
                else
                {
                    serviceCollection.AddTransient(transientType);
                }
            };
        }
        
        private void FillSingleton(IEnumerable<Type> singletonTypes, IServiceCollection serviceCollection)
        {
            foreach(var singletonType in singletonTypes)
            {
                var @interface = GetInterface(singletonType);
                if (@interface is not null)
                {
                    serviceCollection.AddSingleton(@interface, singletonType);
                }
                else
                {
                    serviceCollection.AddSingleton(singletonType);
                }
            };
        }

        private Type? GetInterface(Type type)
        {
            var attribute = type.GetCustomAttribute<UseInterfaceAttribute>();

            if (attribute is not null)
            {
                var @interface = type.GetInterface(attribute.InterfaceName);

                if (@interface is null)
                {
                    throw new RegisterInterfaceException(attribute.InterfaceName);
                }
            }

            return type.GetInterfaces().FirstOrDefault(x=>!_interfaceNames.Contains(x.Name));
        }
        
        /// <summary>
        /// Получить все типы c загруженных
        /// из директории приложения сборок 
        /// </summary>
        /// <returns>Массив всех типов из решения</returns>
        private Type[] GetAllTypes()
        {   
            var projectKey = Options.ProjectKey;

            // получаем все dll-файлы из директории, где собрано приложение
            var files = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase!)
                .GetFiles()
                .Where(x=>x.Name.Contains(projectKey) && x.Name.EndsWith(".dll"));

            // загружаем сборки из файлов
            var assemblies = files
                .Select(x => Assembly.LoadFile(x.FullName));

            // возвращаем все типы, отфильтрованные по названию проекта
            return assemblies.SelectMany(x => x.GetTypes())
                .Where(x => x.FullName?.Contains(projectKey) is true)
                .ToArray();
        }
    }
}