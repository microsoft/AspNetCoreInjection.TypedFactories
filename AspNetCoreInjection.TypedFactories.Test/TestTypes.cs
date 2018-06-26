namespace AspNetCoreInjection.TypedFactories.Test
{
    public interface ITestDependency
    {

    }


    public class TestDependency : ITestDependency
    {

    }

    public interface ITestService
    {
        string FacotoryParam { get; }
        ITestDependency InjectedDepedency { get; }
    }

    public class TestService : ITestService
    {
        public TestService(ITestDependency dep, string factoryParam1)
        {
            this.InjectedDepedency = dep;
            this.FacotoryParam = factoryParam1;
        }

        public string FacotoryParam { get; private set; }
        public ITestDependency InjectedDepedency { get; private set; }
    }

    public interface ITestServiceFlavor1 : ITestService { }

    public class TestServiceFlavor1 : ITestServiceFlavor1
    {
        public TestServiceFlavor1(ITestDependency dep, string factoryParam1)
        {
            this.InjectedDepedency = dep;
            this.FacotoryParam = factoryParam1;
        }

        public string FacotoryParam { get; private set; }
        public ITestDependency InjectedDepedency { get; private set; }
    }

    public interface ITestServiceFlavor2 : ITestService { }

    public class TestServiceFlavor2 : ITestServiceFlavor2
    {
        public TestServiceFlavor2(ITestDependency dep, string factoryParam1)
        {
            this.InjectedDepedency = dep;
            this.FacotoryParam = factoryParam1;
        }

        public string FacotoryParam { get; private set; }
        public ITestDependency InjectedDepedency { get; private set; }
    }

    public interface ITestServiceFactory
    {
        ITestService Create(string factoryParam1);
        ITestServiceFlavor1 CreateFlavor1(string factoryParam1);
        ITestServiceFlavor2 CreateFlavor2(string factoryParam1);
    }

    public interface ITestServiceFactoryBadParamName
    {
        ITestService CreateBadParamName(string badParamName);
    }

    public interface ITestServiceFactoryBadParamType
    {
        ITestService CreateBadParamType(int factoryParam1);
    }

    


    public interface ITestServiceInjectedOnly
    {
        ITestDependency InjectedDepedency { get; }
    }


    public class TestServiceInjectedOnly : ITestServiceInjectedOnly
    {
        public TestServiceInjectedOnly(ITestDependency dep)
        {
            this.InjectedDepedency = dep;
        }

        public string FacotoryParam { get; private set; }
        public ITestDependency InjectedDepedency { get; private set; }
    }

    public interface ITestServiceInjectedOnlyFactory
    {
        ITestServiceInjectedOnly Create();
    }

}
