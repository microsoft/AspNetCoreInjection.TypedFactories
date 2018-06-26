namespace AspNetCoreInjection.TypedFactories
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Defines extension methods for providing custom typed factories based on a factory interface.
    /// </summary>
    public static class AspNetCoreInjectionTypedFactoryExtensions
    {
        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <param name="factoryContractType">
        /// The factory interface.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The holder object which facilitates the fluent interface.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="factoryContractType"/> does not represent an interface type.
        /// </exception>
        public static ITypedFactoryRegistration RegisterTypedFactory(this IServiceCollection container,
                                                                     Type factoryContractType)
        {
            if (!factoryContractType.IsInterface)
            {
                throw new ArgumentException("The factory contract does not represent an interface!", "factoryContractType");
            }

            var typedFactoryRegistration = new TypedFactoryRegistration(container, factoryContractType);
            return typedFactoryRegistration;
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <typeparam name="TFactory">
        /// The factory interface.
        /// </typeparam>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The holder object which facilitates the fluent interface.
        /// </returns>
        public static ITypedFactoryRegistration RegisterTypedFactory<TFactory>(this IServiceCollection container)
            where TFactory : class
        {
            if (!typeof(TFactory).IsInterface)
            {
                throw new ArgumentException("The factory contract does not represent an interface!");
            }

            var typedFactoryRegistration = new TypedFactoryRegistration<TFactory>(container);
            return typedFactoryRegistration;
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="factoryContractType">
        /// The factory interface.
        /// </param>
        /// <param name="toType">
        /// The concrete type that the factory will instantiate.
        /// </param>
        /// <returns>
        /// The container to continue its fluent interface.
        /// </returns>
        public static IServiceCollection RegisterTypedFactory(this IServiceCollection container,
                                                                     Type factoryContractType,
                                                                     Type toType)
        {
            container.RegisterTypedFactory(factoryContractType).ForConcreteType(toType);
            return container;
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <typeparam name="TFactory">
        /// The factory interface.
        /// </typeparam>
        /// <typeparam name="TConcreteType">
        /// The concrete type that the factory will instantiate.
        /// </typeparam>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The container to continue its fluent interface.
        /// </returns>
        public static IServiceCollection RegisterTypedFactory<TFactory, TConcreteType>(this IServiceCollection container)
            where TFactory : class
        {
            return container.RegisterTypedFactory(typeof(TFactory), typeof(TConcreteType));
        }
    }
}