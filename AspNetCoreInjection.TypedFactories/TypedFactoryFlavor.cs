using System;
using System.Collections.Generic;

namespace AspNetCoreInjection.TypedFactories
{
    internal class TypedFactoryFlavor : ITypedFactoryFlavor
    {
        private TypedFactoryRegistration typedFactoryRegistration;
        private Dictionary<Type, Type> flavorMap = new Dictionary<Type, Type>();

        public TypedFactoryFlavor(TypedFactoryRegistration typedFactoryRegistration)
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