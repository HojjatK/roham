using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Roham.Lib.Ioc
{
    public interface IAutoRegistration
    {
        IEnumerable<Assembly> IncludeAssemblies { get; }

        IAutoRegistration IncludeAssembilesFromType(params Type[] assemblyType);
        IAutoRegistration IncludeAssembilesFromTypes(IEnumerable<Type> assemblyType);
        IAutoRegistration IncludeAssembliesFromFiles(IEnumerable<string> assemblyPaths);
        IAutoRegistration IncludeAssembliesFromCurrentDomain(Predicate<Assembly> filter = null);
        IAutoRegistration ExcludeSystemAssemblies();
        IAutoRegistration ExcludeAssemblies(Predicate<Assembly> filter);

        IAutoRegistration IncludeImplementsITypeNameConvention();
        IAutoRegistration IncludeClosingTypeConvention();

        void ApplyRegistrations();
    }

    public class AutoRegistration : IAutoRegistration
    {
        private readonly ICollection<Assembly> _includedAssemblies = new HashSet<Assembly>();
        private readonly ICollection<Predicate<Assembly>> _excludedAssemblyFilters = new List<Predicate<Assembly>>();

        private readonly IRegistrator _registrator;
        private bool includeClosingTypeConvention = false;
        private bool includeImplementsITypeName = false;

        public AutoRegistration(IRegistrator registrator)
        {
            _registrator = registrator;
        }

        public IEnumerable<Assembly> IncludeAssemblies => 
            _includedAssemblies
                    .Where(a => !_excludedAssemblyFilters.Any(f => f(a)))
                    .ToList();

        public IAutoRegistration IncludeAssembilesFromType(params Type[] assemblyType)
        {
            if (assemblyType != null)
            {
                assemblyType
                    .ForEach(t => IncludeAssembly(t.Assembly));
            }
            return this;
        }

        public IAutoRegistration IncludeAssembilesFromTypes(IEnumerable<Type> assemblyType)
        {
            if (assemblyType != null)
            {
                foreach (var t in assemblyType)
                {
                    IncludeAssembly(t.Assembly);
                }
            }
            return this;
        }

        public IAutoRegistration IncludeAssembliesFromFiles(IEnumerable<string> assemblyPaths)
        {
            if (assemblyPaths != null)
            {
                foreach (var assemblyFile in assemblyPaths)
                {
                    IncludeAssembly(Assembly.LoadFrom(assemblyFile));
                }
            }
            return this;
        }

        public IAutoRegistration IncludeAssembliesFromCurrentDomain(Predicate<Assembly> filter = null)
        {
            AppDomain.CurrentDomain.GetAssemblies()
                .ForEach(a =>
                {
                    if (filter == null) IncludeAssembly(a);
                    else if (filter(a)) IncludeAssembly(a);
                });
            return this;
        }

        public IAutoRegistration ExcludeSystemAssemblies()
        {
            ExcludeAssemblies(a =>
            {
                return a.GetName().FullName.StartsWith("System.") ||
                       a.GetName().FullName.StartsWith("mscorlib") ||
                       a.GetName().Name.Equals("System");
            });
            return this;
        }

        public IAutoRegistration ExcludeAssemblies(Predicate<Assembly> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            _excludedAssemblyFilters.Add(filter);
            return this;
        }

        public IAutoRegistration IncludeImplementsITypeNameConvention()
        {
            includeImplementsITypeName = true;
            return this;
        }

        public IAutoRegistration IncludeClosingTypeConvention()
        {
            includeClosingTypeConvention = true;
            return this;
        }

        public void ApplyRegistrations()
        {
            var types = _includedAssemblies
                     .Where(a => !_excludedAssemblyFilters.Any(f => f(a)))
                     .SelectMany(a => a.GetTypes())                     
                     .ToList();

            var openTypes = types.Where(t => t.IsInterface && t.IsGenericTypeDefinition);
            var candidateTypes = types.Where(t => !t.IsInterface && !t.IsAbstract && HasAutoRegistationAttribute(t)).ToList();

            candidateTypes
                .ForEach(type =>
                {
                    if (includeClosingTypeConvention)
                    {
                        ApplyClosingTypeConvention(type, openTypes);
                    }

                    if (includeImplementsITypeName)
                    {
                        ApplyImplementsITypeNameConvention(type);
                    }
                });
        }

        private void IncludeAssembly(Assembly assembly)
        {
            if (!_includedAssemblies.Contains(assembly))
            {
                _includedAssemblies.Add(assembly);
            }
        }

        private void ApplyClosingTypeConvention(Type type, IEnumerable<Type> openTypes)
        {
            foreach (var openType in openTypes)
            {
                if (!type.IsInterface && !type.IsAbstract)
                {
                    // Retrieve interfaces which match the configured open type
                    Type firstClosingInterface =
                        type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition().Equals(openType));
                    
                    if (firstClosingInterface != null)
                    {
                        // Register open generic type implementations (e.g. Repository<T> : IRepository<T>) as the configured open type,
                        // otherwise the type is closed (e.g. PersonRepository : IRepository<Person>) and should be registered
                        // as the first closing interface matching the open type definition.
                        if (type.IsOpenGeneric())
                        {
                            RegisterBaseOnLifetimeScope(openType, type);
                        }
                        else
                        {
                            RegisterBaseOnLifetimeScope(firstClosingInterface, type);
                        }
                    }
                }
            }
        }

        private void ApplyImplementsITypeNameConvention(Type type)
        {
            var interfaceType = type
                .GetInterfaces()
                .SingleOrDefault(i => i.Name.StartsWith("I") && i.Name.Remove(0, 1) == type.Name);
            if (interfaceType != null)
            {
                RegisterBaseOnLifetimeScope(interfaceType, type);
            }
        }

        private bool HasAutoRegistationAttribute(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(AutoRegisterAttribute), false);
            return attrs != null && attrs.Any();
        }

        private void RegisterBaseOnLifetimeScope(Type interfaceType, Type implementerType)
        {
            var regInfo = implementerType.GetRegistrationInfoFromCustomAttribues();
            var lifeTimeScope = regInfo.Item1;
            var name = regInfo.Item2;

            switch (lifeTimeScope)
            {
                case LifetimeScopeType.SingleInstance:
                    _registrator.RegisterAsSingleInstance(interfaceType, implementerType, name);
                    break;
                case LifetimeScopeType.InstancePerLifetimeScope:
                    _registrator.RegisterAsPerLifetimeScope(interfaceType, implementerType, name);
                    break;
                case LifetimeScopeType.InstancePerRequest:
                    _registrator.RegisterAsPerRequest(interfaceType, implementerType, name);
                    break;
                case LifetimeScopeType.InstancePerDependency:
                default:
                    _registrator.Register(interfaceType, implementerType, name);
                    break;
            }
        }
    }
}