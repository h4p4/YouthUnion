namespace YouthUnion
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Microsoft.EntityFrameworkCore;

    internal static class ContextProvider
    {
        public static YouthUnionContext YouthUnionContext { get; } = new();

        public static IQueryable Set(Type T)
        {
            // Get the generic type definition
            //var method =
            //    typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);
            var method = typeof(DbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance).First(m => m.Name == nameof(DbContext.Set) && !m.GetParameters().Any());
            // Build a method with the specific type argument you're interested in
            method = method.MakeGenericMethod(T);

            return method.Invoke(YouthUnionContext, null) as IQueryable;
        }
    }
}