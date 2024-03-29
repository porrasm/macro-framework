﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MacroFramework.Tools {
    internal static class ReflectiveEnumerator {
        static ReflectiveEnumerator() { }

        internal static IEnumerable<T> GetEnumerableOfType<T>(Assembly assembly) where T : class {
            List<T> objects = new List<T>();
            foreach (Type type in
                GetLoadableTypes(assembly)
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))) {
                objects.Add((T)Activator.CreateInstance(type));
            }
            return objects;
        }
        internal static IEnumerable<Type> GetLoadableTypes(Assembly assembly) {
            try {
                return assembly.GetTypes();
            } catch (ReflectionTypeLoadException e) {
                return e.Types.Where(t => t != null);
            }
        }

        internal static List<T> GetCommands<T>() {

            try {
                List<T> ts = new List<T>();
                Type type = typeof(T);

                var sub_validator_types = type
                    .Assembly
                    .DefinedTypes
                    .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract)
                    .ToList();


                foreach (var sub_validator_type in sub_validator_types) {
                    T sub_validator = (T)Activator.CreateInstance(sub_validator_type);
                    ts.Add(sub_validator);
                }
                return null;

            } catch (Exception e) {
                Logger.Log("err " + e.Message);
                throw;
            }
        }
    }
}
