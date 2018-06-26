namespace AspNetCoreInjection.TypedFactories
{
    using System;
    using System.Reflection;

    using Castle.DynamicProxy;
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;

    internal class TypedFactoryRegistration<TFactory> : TypedFactoryRegistration
        where TFactory : class
    {
        public TypedFactoryRegistration(IServiceCollection container) : base(container, typeof(TFactory))
        {
        }

        /// <summary>
        /// Defines the concrete type which the factory will create.
        /// </summary>
        /// <typeparam name="TTo">
        /// The concrete type which the factory will instantiate.
        /// </typeparam>
        public override void ForConcreteType<TTo>()
        {
            ForConcreteType(typeof(TTo));
        }
    }

    /// <summary>
    /// Implements the fluent interface for registering typed factories.
    /// </summary>
    public class TypedFactoryRegistration : ITypedFactoryRegistration
    {
        /// <summary>
        /// The Castle proxy generator.
        /// </summary>
        private static readonly Lazy<ProxyGenerator> LazyProxyGenerator = new Lazy<ProxyGenerator>();
        
        /// <summary>
        ///     The factory interface.
        /// </summary>
        private readonly Type factoryContractType;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedFactoryRegistration"/> class.
        /// </summary>
        /// <param name="container">
        ///     The target container on which to perform the registrations.
        /// </param>
        /// <param name="factoryContractType">
        ///     The factory interface.
        /// </param>
        /// <param name="name">
        ///     Name that will be used to request the type.
        /// </param>
        public TypedFactoryRegistration(IServiceCollection container, Type factoryContractType)
        {
            this.factoryContractType = factoryContractType;
            this.Container = container;
        }

        public IServiceCollection Container { get; private set; }

        /// <summary>
        /// Gets the Castle proxy generator. A new instance will be created upon the first access, and reused afterwards.
        /// </summary>
        protected static ProxyGenerator ProxyGenerator
        {
            get
            {
                return LazyProxyGenerator.Value;
            }
        }

        /// <summary>
        /// Defines the concrete type which the factory will create.
        /// </summary>
        /// <param name="toType">
        /// The concrete type which the factory will instantiate.
        /// </param>
        public void ForConcreteType(Type toType)
        {
            Func<IServiceProvider, object> factoryFunc = new Func<IServiceProvider, object>
            (serviceProvider =>
            {
                var interceptor = new FactoryInterceptor(serviceProvider);
                interceptor.SetSingleConcreteType(toType);
                VerifyFactoryMethods(interceptor);
                return ProxyGenerator.CreateInterfaceProxyWithoutTarget(this.factoryContractType, interceptor);
            });

            this.Container.AddSingleton(this.factoryContractType, factoryFunc);
        }

        public void ForConcreteTypes(Dictionary<Type, Type> mapping)
        {
            Func<IServiceProvider, object> factoryFunc = new Func<IServiceProvider, object>
            (serviceProvider =>
            {
                var interceptor = new FactoryInterceptor(serviceProvider);
                foreach(var pair in mapping)
                {
                    interceptor.AddConcreteTypeMapping(pair.Key, pair.Value);
                }

                VerifyFactoryMethods(interceptor);
                return ProxyGenerator.CreateInterfaceProxyWithoutTarget(this.factoryContractType, interceptor);
            });

            this.Container.AddSingleton(this.factoryContractType, factoryFunc);
        }

        public ITypedFactoryFlavor Flavor<TFrom, TTo>()
        {
            var flavor = new TypedFactoryFlavor(this);
            flavor.Flavor<TFrom, TTo>();
            return flavor;
        }

        private void VerifyFactoryMethods(FactoryInterceptor interceptor)
        {
            var factoryMethods = this.factoryContractType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var factoryMethod in factoryMethods)
            {
                interceptor.VerifyFactorySignature(factoryMethod);
            }
        }

        /// <summary>
        /// Defines the concrete type which the factory will create.
        /// </summary>
        /// <typeparam name="TTo">
        /// The concrete type which the factory will instantiate.
        /// </typeparam>
        public virtual void ForConcreteType<TTo>()
        {
            this.ForConcreteType(typeof(TTo));
        }
    }
}