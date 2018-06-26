namespace AspNetCoreInjection.TypedFactories
{
    public interface ITypedFactoryFlavor
    {
        ITypedFactoryFlavor Flavor<TFrom, TTo>();

        void Register();
    }
}