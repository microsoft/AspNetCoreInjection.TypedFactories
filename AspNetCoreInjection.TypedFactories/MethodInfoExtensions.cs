using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AspNetCoreInjection.TypedFactories
{
    public static class MethodInfoExtensions
    {
        public static string FullName(this MethodInfo method)
        {
            return $"{method.ReflectedType.FullName}::{method.Name}";
        }
    }
}
