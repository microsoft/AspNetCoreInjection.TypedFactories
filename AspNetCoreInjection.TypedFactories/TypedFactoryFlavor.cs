using System;
using System.Collections.Generic;

namespace AspNetCoreInjection.TypedFactories
{
    internal class TypedFactoryFlavor<TFactory> : ITypedFactoryFlavor where TFactory : class
    {
        private readonly TypedFactoryRegistration<TFactory> typedFactoryRegistration;
        private readonly Dictionary<Type, Type> flavorMap = new Dictionary<Type, Type>();

        public TypedFactoryFlavor(TypedFactoryRegistration<TFactory> typedFactoryRegistration)
        {
            this.typedFactoryRegistration = typedFactoryRegistration;
        }

        public ITypedFactoryFlavor Flavor<TFrom, TTo>()
        {
            this.flavorMap.Add(typeof(TFrom), typeof(TTo));
            return this;
        }

        public void Register()
        {
            this.typedFactoryRegistration.ForConcreteTypes(flavorMap);
        }
    }
}