namespace AspNetCoreInjection.TypedFactories
{
    using Castle.DynamicProxy;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines an <see cref="IInterceptor"/> implementation which implements the factory methods, 
    /// by passing the method arguments by name into a specified concrete type's constructor.
    /// </summary>
    internal class FactoryInterceptor : IInterceptor
    {
        private readonly IServiceProvider container;
        Dictionary<Type, Resolver> resolversMap = new Dictionary<Type, Resolver>();
        private Resolver singleResolver;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FactoryInterceptor"/> class.
        /// </summary>
        public FactoryInterceptor(IServiceProvider container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }

        /// <summary>
        ///     Adds a specific mapping between interface and concrete class. Used when the factory class can resolve multiple concrete classes
        /// </summary>
        /// <param name="from">The interface</param>
        /// <param name="to">The concrete type</param>
        public void AddConcreteTypeMapping(Type from, Type to)
        {
            if (!from.IsInterface)
            {
                throw new Exception("From is expected to be an interface");
            }

            if (!to.IsClass)
            {
                throw new Exception("To is expected to be concrete class");
            }

            this.resolversMap.Add(from, new Resolver(to, container));
        }

        /// <summary>
        ///     Overrides specific mappings and registers a single concrete type
        /// </summary>
        /// <param name="to">The single concerete type this factory can create</param>
        public void SetSingleConcreteType(Type to)
        {
            if (!to.IsClass)
            {
                throw new Exception("To is expected to be concrete class");
            }

            this.singleResolver = new Resolver(to, container);
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method == null)
            {
                throw new ArgumentNullException("invocation.Method");
            }

            invocation.ReturnValue = GetResolver(invocation.Method).ResolveWithArguments(invocation.Method, invocation.Arguments);
        }

        public void VerifyFactorySignature(MethodInfo factoryMethod)
        {
            GetResolver(factoryMethod).VerifyFactorySignature(factoryMethod);
        }

        /// <summary>
        ///     Returns the matching resolver for the factory method. If a singleResolver is registered, that resolver is always returned
        ///     If there is no singleResolver, the resolverMap is used to lookup a resolver based on the method return type
        /// </summary>
        private Resolver GetResolver(MethodInfo factoryMethod)
        {
            Resolver resolver = null;
            if (singleResolver != null)
            {
                resolver = singleResolver;
            }
            else
            {
                if (!this.resolversMap.TryGetValue(factoryMethod.ReturnType, out resolver))
                {
                    var methodName = $"{factoryMethod.ReflectedType.FullName}::{ factoryMethod.Name}";
                    throw new Exception($"Factory method {methodName} returns interface {factoryMethod.ReturnType.FullName} which doesn't have a registered flavor.");
                }
            }

            return resolver;
        }
    }
}