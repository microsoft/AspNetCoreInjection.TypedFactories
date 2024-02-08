using Xunit;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Xunit.Abstractions;

namespace AspNetCoreInjection.TypedFactories.Test
{
    public class PerformanceTests
    {
        private readonly ITestOutputHelper testOutput;

        public PerformanceTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }
        [Fact(Skip = "Performance measurement")]
        public void MeasurePerformance()
        {
            IServiceCollection container = new ServiceCollection();
            container.AddTransient<ITestDependency, TestDependency>();
            container.AddTransient<ITestService, TestService>();
            container.AddTransient<string>(sp => "aaa");
            container.RegisterTypedFactory<ITestServiceFactory>().ForConcreteType<TestService>();

            using (var svcProvider = container.BuildServiceProvider())
            {

                //Warmup
                svcProvider.GetRequiredService<ITestServiceFactory>().Create("ParamValue");

                const int N = 100000;
                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < N; i++)
                {
                    svcProvider.GetRequiredService<ITestServiceFactory>().Create("ParamValue");
                }
                testOutput.WriteLine($"Factory: {sw.ElapsedMilliseconds}");

                sw = Stopwatch.StartNew();
                for (int i = 0; i < N; i++)
                {
                    svcProvider.GetRequiredService<ITestService>();
                }
                testOutput.WriteLine($"ServiceCollection: {sw.ElapsedMilliseconds}");
            }

        }
    }
}
