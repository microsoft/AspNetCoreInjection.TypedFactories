using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCoreInjection.TypedFactories
{
    /// <summary>
    /// Defines the contract for the fluent interface for registering typed factories.
    /// </summary>
    public interface ITypedFactoryRegistration
    {

        /// <summary>
        /// Defines the concrete type which the factory will create.
        /// </summary>
        /// <typeparam name="TTo">
        /// The concrete type which the factory will instantiate.
        /// </typeparam>
        void ForConcreteType<TTo>();

        /// <summary>
        /// Maps specific interface <see cref="TFrom"/> to concrete class <see cref="TTo"/>
        /// </summary>
        /// <typeparam name="TFrom">Interface that the factory method returns</typeparam>
        /// <typeparam name="TTo">The concrete type which the factory will instantiate.</typeparam>
        ITypedFactoryFlavor Flavor<TFrom, TTo>();
    }
}