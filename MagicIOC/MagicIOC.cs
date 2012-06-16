﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace MagicIOC
{
    /// <summary>
    /// 
    /// </summary>
    public static class MagicIOC
    {
        // The cache of the instances we have created
        private static readonly ConcurrentDictionary<Type, object> _instanceCache = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Creates and returns an instance of the given type, if its dependencies can be satisfied.
        /// </summary>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <returns>An instance of T</returns>
        public static T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        private static object Get(Type type)
        {
            object instance = null;

            if (!_instanceCache.TryGetValue(type, out instance))
            {
                if (type.IsClass)
                {
                    // If we don't have any constructors
                    var constructors = type.GetConstructors();
                    if (!constructors.Any())
                        throw new ArgumentException(String.Format("The type {0} does not have any accessible constructors", type.FullName));

                    if (constructors.Any(c => !c.GetParameters().Any()))
                    {
                        // If we have a parameterless constructor, use it
                        instance = Activator.CreateInstance(type);
                    }
                    else
                    {
                        // Otherwise, try to find a constructor we can use
                        instance = CreateInstanceForType(type);
                    }
                }
                else if (type.IsInterface)
                {
                    instance = FindImplementationOfInterface(type);
                    if (instance == null)
                        throw new ArgumentException(String.Format("The interface {0} does not have any suitable implementations", type.FullName));
                }

                // If creation was successful, add it to our cache
                if (instance != null)
                    _instanceCache.TryAdd(type, instance);
            }

            return instance;
        }

        private static object FindImplementationOfInterface(Type type)
        {
            object instance = null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var assemblyType in assembly.GetTypes())
                {
                    if (type != assemblyType && type.IsAssignableFrom(assemblyType))
                    {
                        object p = Get(assemblyType);
                        if (p != null)
                        {
                            instance = p;
                            break;
                        }
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Attempts to create an instance of the specified type by examining its constructors
        /// and using the first one that has resolveable dependencies.
        /// </summary>
        /// <param name="type">The type we want to create</param>
        /// <returns>An instance of the given type, or null if its dependencies could not be satisfied</returns>
        private static object CreateInstanceForType(Type type)
        {
            object instance = null;

            foreach (var constructor in type.GetConstructors())
            {
                // See if we can create an instance for this constructor. If we can, we're done.
                instance = CreateInstanceWithConstructor(constructor);
                if (instance != null)
                    break;
            }

            return instance;
        }

        /// <summary>
        /// Attempts to create an instance of an object using the specified constructor.
        /// Checks each constructor parameter to make sure that it can be resolved.
        /// </summary>
        /// <param name="constructor">The constructor that we are attempting to invoke</param>
        /// <returns>An object created by the constructor, or null if the dependencies could not be satisfied</returns>
        private static object CreateInstanceWithConstructor(System.Reflection.ConstructorInfo constructor)
        {
            object instance = null;

            var parameters = constructor.GetParameters();
            var parameterValues = new List<object>();

            // Loop through each parameter and see if we can resolve it
            foreach (var parameter in parameters)
            {
                object p = Get(parameter.ParameterType);
                if (p == null)
                    break;

                parameterValues.Add(p);
            }

            // If we were able to resolve each parameter, create the instance
            if (parameterValues.Count == parameters.Count())
                instance = Activator.CreateInstance(constructor.DeclaringType, parameterValues.ToArray());

            return instance;
        }
    }
}