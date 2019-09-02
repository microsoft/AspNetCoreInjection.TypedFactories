using Xunit;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace AspNetCoreInjection.TypedFactories.Test
{
    public class TypedFactoryTest
    {
        [Fact]
        public void ResolveWithInjectedAndInvokerParameters()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceFactory>().ForConcreteType<TestService>();

            using (var svcProvider = container.BuildServiceProvider())
            {
                ITestService testSvc = svcProvider.GetRequiredService<ITestServiceFactory>().Create("ParamValue");

                Assert.Equal("ParamValue", testSvc.FacotoryParam);
                Assert.NotNull(testSvc.InjectedDepedency);
            }
        }

        [Fact]
        public void ResolveFlavor()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceFactory>()
                .Flavor<ITestService, TestService>()
                .Flavor<ITestServiceFlavor1, TestServiceFlavor1>()
                .Flavor<ITestServiceFlavor2, TestServiceFlavor2>()
                .Register();

            using (var svcProvider = container.BuildServiceProvider())
            {
                ITestService f1 = svcProvider.GetRequiredService<ITestServiceFactory>().CreateFlavor1("Flavor1");
                ITestService f2 = svcProvider.GetRequiredService<ITestServiceFactory>().CreateFlavor2("Flavor2");

                Assert.IsAssignableFrom<ITestServiceFlavor1>(f1);
                Assert.Equal("Flavor1", f1.FacotoryParam);

                Assert.IsAssignableFrom<ITestServiceFlavor2>(f2);
                Assert.Equal("Flavor2", f2.FacotoryParam);
            }
        }

        [Fact]
        public void ResolveWithInjectedParametersOnly()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceInjectedOnlyFactory>().ForConcreteType<TestServiceInjectedOnly>();

            using (var svcProvider = container.BuildServiceProvider())
            {
                ITestServiceInjectedOnly testSvc = svcProvider.GetRequiredService<ITestServiceInjectedOnlyFactory>().Create();

                Assert.NotNull(testSvc.InjectedDepedency);
            }
        }

        [Fact]
        public void ResolveWithMissingDependecy()
        {
            IServiceCollection container = new ServiceCollection();
            container.RegisterTypedFactory<ITestServiceFactory>().ForConcreteType<TestService>();

            using (var svcProvider = container.BuildServiceProvider())
            {
                Assert.Throws<InvalidOperationException>(() =>
                    svcProvider.GetRequiredService<ITestServiceFactory>().Create("ParamValue")
                );
            }
        }

        [Fact]
        public void ResolveBadParameterName()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceFactoryBadParamName>().ForConcreteType<TestService>();

            using (var svcProvider = container.BuildServiceProvider())
            {
                Assert.Throws<Exception>(() =>
                    svcProvider.GetRequiredService<ITestServiceFactoryBadParamName>().CreateBadParamName("BadParamValue")
                );
            }
        }

        [Fact]
        public void ResolveBadParameterType()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.RegisterTypedFactory<ITestServiceFactoryBadParamType>().ForConcreteType<TestService>();

            using (var svcProvider = container.BuildServiceProvider())
            {
                Assert.Throws<Exception>(() =>
                    svcProvider.GetRequiredService<ITestServiceFactoryBadParamType>().CreateBadParamType(1)
                );
            }
        }



    }
}
