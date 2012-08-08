using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace QuickCheck.SimpleInjector
{
    public static class RegistrationExtensions
    {
        public static void RegisterManySingles(this Container container, Type genericType, Assembly assembly)
        {
            var registrations = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract)
                .SelectMany(t => t.GetInterfaces().Select(i => new { impl = t, service = i }))
                .Where(x => genericType.GetGenericArguments(x.service) != null)
                .ToArray();

            foreach (var registration in registrations)
            {
                container.RegisterAnySingle(registration.service, registration.impl);
            }
        }

        public static void RegisterAnySingle(this Container container, Type service, Type implementation)
        {
            if (!service.ContainsGenericParameters && !implementation.ContainsGenericParameters)
            {
                container.RegisterSingle(service, implementation);
                return;
            }

            container.ResolveUnregisteredType += (sender, e) =>
            {
                var arguments = service.GetGenericArguments(e.UnregisteredServiceType);

                if (arguments == null)
                {
                    return;
                }

                Type specific = implementation.BindGenericArguments(arguments);

                if (specific.ContainsGenericParameters)
                {
                    return;
                }

                e.Register(container.GetRegistration(specific).BuildExpression());
            };
        }

        private static Type BindGenericArguments(this Type type, Dictionary<Type, Type> bindings)
        {
            Type restricted;
            if (bindings.TryGetValue(type, out restricted))
            {
                return restricted;
            }
            
            if (type.IsArray)
            {
                return type.GetElementType().BindGenericArguments(bindings).MakeArrayType();
            }

            if (type.IsGenericType)
            {
                Type[] args = type.GetGenericArguments().Select(x => x.BindGenericArguments(bindings)).ToArray();
                return type.GetGenericTypeDefinition().MakeGenericType(args);
            }

            return type;
        }

        private static Dictionary<Type, Type> GetGenericArguments(this Type generic, Type instance)
        {
            var bindings = new Dictionary<Type, Type>();

            if (bindings.AddBindings(generic, instance))
            {
                return bindings;
            }

            return null;
        }

        private static bool AddBindings(
            this Dictionary<Type, Type> bindings, Type generic, Type instance)
        {
            if (generic.IsAssignableFrom(instance) || generic.IsGenericParameter)
            {
                bindings.Add(generic, instance);
            }
            else if (generic.IsArray)
            {
                if (!instance.IsArray)
                {
                    return false;
                }

                if (!bindings.AddBindings(generic.GetElementType(), instance.GetElementType()))
                {
                    return false;
                }
            }
            else if (generic.IsGenericType)
            {
                if (!instance.IsGenericType || generic.GetGenericTypeDefinition() != instance.GetGenericTypeDefinition())
                {
                    return false;
                }

                Type[] genericArgs = generic.GetGenericArguments();
                Type[] instanceArgs = instance.GetGenericArguments();

                for (int i = 0; i < instanceArgs.Length; i++)
                {
                    if (!bindings.AddBindings(genericArgs[i], instanceArgs[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
