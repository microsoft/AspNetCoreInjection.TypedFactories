using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AspNetCoreInjection.TypedFactories
{
    /// <summary>
    ///     Handles the actual resoltion to a concrete class
    /// </summary>
    public class Resolver
    {
        private readonly Type concreteType;
        private readonly IServiceProvider container;

        Func<object[], object> ctorInvokeFunc;
        ParameterInfo[] ctorParameters;

        public Resolver(Type concreteType, IServiceProvider container)
        {
            this.concreteType = concreteType;
            this.container = container;

            var ctors = concreteType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (ctors.Length != 1)
            {
                throw new Exception($"Class {concreteType.FullName} has {ctors.Length} public c-tors. It needs to have exactly 1");
            }

            var ctor = ctors[0];

            this.ctorInvokeFunc = CreateCtorInvokeFunc(ctor);
            this.ctorParameters = ctor.GetParameters();
        }

        /// <summary>
        /// Create a compliled lambda expression which calls the ctor. It is more effiecient than calling ctor.Invoke();
        /// </summary>
        Func<object[], object> CreateCtorInvokeFunc(ConstructorInfo ctor)
        {
            Type[] types = ctor.GetParameters().Select(p => p.ParameterType).ToArray();

            var args = Expression.Parameter(typeof(object[]), "args");
            var body = Expression.New(ctor,
                types.Select((t, i) => Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(i)), t)).ToArray());
            var outer = Expression.Lambda<Func<object[], object>>(body, args);
            Func<object[], object> ctorInvokeFunc = outer.Compile();
            return ctorInvokeFunc;
        }

        /// <summary>
        ///     Verify that the signature of the factory method matches the parameters of the ctor
        ///     Only do this once per method
        /// </summary>
        public void VerifyFactorySignature(MethodInfo factoryMethod)
        {
            var factoryMethodParams = factoryMethod.GetParameters();
            foreach (var p in factoryMethodParams)
            {
                var methodName = factoryMethod.FullName();

                var ctorParam = ctorParameters.FirstOrDefault(x => x.Name == p.Name);
                if (ctorParam == null)
                {
                    throw new Exception($"Factory method {methodName} declares parameter {p.Name} which doesn't exist in the ctor for {concreteType.FullName}");
                }

                if (ctorParam.ParameterType != p.ParameterType)
                {
                    throw new Exception($"Factory method {methodName} declares parameter {p.Name} which is of different type from the one declared in the ctor of {concreteType.FullName}");
                }
            }

            VerifyDependecies(factoryMethodParams.Select(p => p.Name).ToList());
        }


        /// <summary>
        ///     Verifies that all dependecies of <see cref="concreteType"/> are registered and are resolvable
        /// </summary>
        void VerifyDependecies(List<string> excludedFactoryParams)
        {
            foreach (var ctorParam in this.ctorParameters)
            {
                if (excludedFactoryParams.Contains(ctorParam.Name))
                    continue;

                this.container.GetRequiredService(ctorParam.ParameterType);
            }
        }

        public object ResolveWithArguments(MethodInfo factoryMethod, object[] args)
        {
            if (!factoryMethod.ReturnType.IsAssignableFrom(this.concreteType))
            {
                throw new Exception($"The concrete type {this.concreteType.FullName} does not implement the factory method {factoryMethod.FullName()} return type {factoryMethod.ReturnType}");
            }

            Dictionary<string, object> factoryMethodArgs = new Dictionary<string, object>();
            var factoryMethodParams = factoryMethod.GetParameters();
            for (int i = 0; i < factoryMethodParams.Length; i++)
            {
                factoryMethodArgs.Add(factoryMethodParams[i].Name, args[i]);
            }

            var ctorParamValues = ctorParameters.Select(p => ResolveParameter(p, factoryMethodArgs)).ToArray();
            //return ctor.Invoke(ctorParamValues);
            return this.ctorInvokeFunc(ctorParamValues);
        }

        private object ResolveParameter(ParameterInfo resolvedCtorParam, Dictionary<string, object> factoryMethodArgs)
        {
            object resolvedParamValue;
            if (factoryMethodArgs.ContainsKey(resolvedCtorParam.Name))
            {
                resolvedParamValue = factoryMethodArgs[resolvedCtorParam.Name];
            }
            else
            {
                resolvedParamValue = this.container.GetRequiredService(resolvedCtorParam.ParameterType);
            }
            return resolvedParamValue;
        }

    }
}
