namespace AspNetCoreInjection.TypedFactories
{
    using System;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;


    /// <summary>
    /// Implements the fluent interface for registering typed factories.
    /// </summary>
    public class TypedFactoryRegistration<TFactory> : ITypedFactoryRegistration where TFactory : class
    {
        private readonly IServiceCollection container;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedFactoryRegistration"/> class.
        /// </summary>
        /// <param name="container">
        ///     The target container on which to perform the registrations.
        /// </param>
        public TypedFactoryRegistration(IServiceCollection container)
        {
            this.container = container;
        }

        public void ForConcreteTypes(Dictionary<Type, Type> mapping)
        {

            this.container.AddSingleton(serviceProvider =>
            {
                TFactory factory = DispatchProxy.Create<TFactory, FactoryInterceptor>();

                if (factory is FactoryInterceptor interceptor)
                {
                    interceptor.Initialize(serviceProvider);

                    foreach (var pair in mapping)
                    {
                        interceptor.AddConcreteTypeMapping(pair.Key, pair.Value);
                    }
                    VerifyFactoryMethods(interceptor);
                }

                return factory;

            });

        }


        /// <summary>
        /// Defines the concrete type which the factory will create.
        /// </summary>
        /// <typeparam name="TTo">
        /// The concrete type which the factory will instantiate.
        /// </typeparam>
        public void ForConcreteType<TTo>()
        {
            this.container.AddSingleton(serviceProvider =>
            {
                TFactory factory = DispatchProxy.Create<TFactory, FactoryInterceptor>();

                if (factory is FactoryInterceptor interceptor)
                {
                    interceptor.Initialize(serviceProvider);
                    interceptor.SetSingleConcreteType(typeof(TTo));
                    VerifyFactoryMethods(interceptor);
                }

                return factory;

            });
        }

        /// <summary>
        /// Registers implementations for factory methods
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <returns></returns>
        public ITypedFactoryFlavor Flavor<TFrom, TTo>()
        {
            return new TypedFactoryFlavor<TFactory>(this).Flavor<TFrom, TTo>();
        }

        private void VerifyFactoryMethods(FactoryInterceptor interceptor)
        {
            var factoryMethods = typeof(TFactory).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var factoryMethod in factoryMethods)
            {
                interceptor.VerifyFactorySignature(factoryMethod);
            }
        }

    }
}
