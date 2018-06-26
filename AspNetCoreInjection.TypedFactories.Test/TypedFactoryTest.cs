using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace AspNetCoreInjection.TypedFactories.Test
{
    [TestClass]
    public class TypedFactoryTest
    {
        [TestMethod]
        public void ResolveWithInjectedAndInvokerParameters()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceFactory>().ForConcreteType<TestService>();

            var svcProvider = container.BuildServiceProvider();
            ITestService testSvc = svcProvider.GetRequiredService<ITestServiceFactory>().Create("ParamValue");

            Assert.AreEqual("ParamValue", testSvc.FacotoryParam);
            Assert.IsNotNull(testSvc.InjectedDepedency);
        }

        [TestMethod]
        public void ResolveFlavor()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceFactory>()
                .Flavor<ITestService, TestService>()
                .Flavor<ITestServiceFlavor1, TestServiceFlavor1>()
                .Flavor<ITestServiceFlavor2, TestServiceFlavor2>()
                .Register();

            var svcProvider = container.BuildServiceProvider();
            ITestService f1 = svcProvider.GetRequiredService<ITestServiceFactory>().CreateFlavor1("Flavor1");
            ITestService f2 = svcProvider.GetRequiredService<ITestServiceFactory>().CreateFlavor2("Flavor2");

            Assert.IsInstanceOfType(f1, typeof(ITestServiceFlavor1));
            Assert.AreEqual("Flavor1", f1.FacotoryParam);

            Assert.IsInstanceOfType(f2, typeof(ITestServiceFlavor2));
            Assert.AreEqual("Flavor2", f2.FacotoryParam);
        }

        [TestMethod]
        public void ResolveWithInjectedParametersOnly()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceInjectedOnlyFactory>().ForConcreteType<TestServiceInjectedOnly>();

            var svcProvider = container.BuildServiceProvider();
            ITestServiceInjectedOnly testSvc = svcProvider.GetRequiredService<ITestServiceInjectedOnlyFactory>().Create();

            Assert.IsNotNull(testSvc.InjectedDepedency);
        }

        [TestMethod]
        public void ResolveWithMissingDependecy()
        {
            IServiceCollection container = new ServiceCollection();
            container.RegisterTypedFactory<ITestServiceFactory>().ForConcreteType<TestService>();

            var svcProvider = container.BuildServiceProvider();
            Assert.ThrowsException<InvalidOperationException>(() =>
                svcProvider.GetRequiredService<ITestServiceFactory>().Create("ParamValue")
            );
        }

        [TestMethod]
        public void ResolveBadParameterName()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceFactoryBadParamName>().ForConcreteType<TestService>();

            var svcProvider = container.BuildServiceProvider();
            Assert.ThrowsException<Exception>(() =>
                svcProvider.GetRequiredService<ITestServiceFactoryBadParamName>().CreateBadParamName("BadParamValue")
            );
        }

        [TestMethod]
        public void ResolveBadParameterType()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceFactoryBadParamType>().ForConcreteType<TestService>();

            var svcProvider = container.BuildServiceProvider();
            Assert.ThrowsException<Exception>(() =>
                svcProvider.GetRequiredService<ITestServiceFactoryBadParamType>().CreateBadParamType(1)
            );
        }

        [TestMethod]
        [Ignore]
        public void MeasurePerformance()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.AddTransient<ITestService, TestService>();
            container.AddTransient<string>(sp => "aaa");
            container.RegisterTypedFactory<ITestServiceFactory>().ForConcreteType<TestService>();

            var svcProvider = container.BuildServiceProvider();
            
            //Warmup
            svcProvider.GetRequiredService<ITestServiceFactory>().Create("ParamValue");

            const int N = 100000;
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < N; i++)
            {
                svcProvider.GetRequiredService<ITestServiceFactory>().Create("ParamValue");
            }
            Console.WriteLine($"Factory: {sw.ElapsedMilliseconds}");

            sw = Stopwatch.StartNew();
            for (int i = 0; i < N; i++)
            {
                svcProvider.GetRequiredService<ITestService>();
            }
            Console.WriteLine($"ServiceCollection: {sw.ElapsedMilliseconds}");

        }

    }
}
