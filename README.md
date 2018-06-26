# AspNetCoreInjection.TypedFactories

Based on https://github.com/PombeirP/Unity.TypedFactories

This project provides automatic Automatic Factory functionality similar to Castle.Windsor Typed Factories, for the default IoC container in ASP.NET Core (ServiceCollection)

Usage
-----

    IServiceCollection container = new ServiceCollection();
    container.AddTransient<ITestDependency, TestDependency>();
    container.RegisterTypedFactory<ITestServiceFactory>().ForConcreteType<TestService>();

    var svcProvider = container.BuildServiceProvider();
    ITestService testSvc = svcProvider.GetRequiredService<ITestServiceFactory>().Create("ParamValue");

    ...   

    public class TestService : ITestService
    {
        public TestService(ITestDependency dep, string factoryParam1)
        {
            this.InjectedDepedency = dep;
            this.FacotoryParam = factoryParam1;
        }
    }

    public interface ITestServiceFactory
    {
        ITestService Create(string factoryParam1);
    }

Sometimes you want to have a single factory which can create multiple concrete types. Usually the concrete classes are different implementations of the same interface. 

    container.RegisterTypedFactory<ITestServiceFactory>()
        .Flavor<ITestServiceFlavor1, TestServiceFlavor1>()
        .Flavor<ITestServiceFlavor2, TestServiceFlavor2>()
        .Register();

    ...

    public interface ITestServiceFlavor1 : ITestService { }
    public class TestServiceFlavor1 : ITestServiceFlavor1
    {
        public TestServiceFlavor1(ITestDependency dep, string factoryParam1)
        {
        }
    }

    public interface ITestServiceFlavor2 : ITestService { }
    public class TestServiceFlavor2 : ITestServiceFlavor2
    {
        public TestServiceFlavor2(ITestDependency dep, string factoryParam1)
        {
        }
    }

    public interface ITestServiceFactory
    {
        ITestServiceFlavor1 CreateFlavor1(string factoryParam1);
        ITestServiceFlavor2 CreateFlavor2(string factoryParam1);
    }



Contract
-----
The concrete type must have exactly one public constructor. The factory parameters must match the constructor parameters by name and type. 

The library will validate all factory methods when the factory instance is constructed. An exception will be thrown if:
- The factory method has parameters that don't match the constructor parameters
- The constructor expects parameters that aren't provided by the factory method and can't be resolved by the container

This is done during factory construction in order to fail fast if something is wrong, instead of failing when the bad code is executed.


# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
