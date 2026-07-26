using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BBUnity.Entities.Controllers.Base {

    public interface IEntityController { }

    /// <summary>
    /// The base controller for all entity controllers. 
    /// </summary>
    public class EntityController : MonoBehaviour, IEntityController {

        private const BindingFlags ReflectedFieldBindingFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

        // The set of matching FieldInfo for a given (concrete type, target type) pair can never
        // change at runtime, so it's computed once and shared across every instance of that
        // type (e.g. pooled enemies) instead of re-walking the type hierarchy via reflection on
        // every Awake().
        private static readonly Dictionary<(Type Owner, Type Target), FieldInfo[]> _reflectedFieldCache = new();

        /// <summary>
        /// The virtual Initalize method for all controllers. This can be overridden
        /// by the developer
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// Finds a given set of fields via Reflection whose type is exactly T, a subclass of T,
        /// or (when T is an interface) an implementation of T. Fields that are still unassigned
        /// (null) are skipped rather than included as a null entry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T[] FindReflectedFields<T>() {
            var cacheKey = (GetType(), typeof(T));

            if (!_reflectedFieldCache.TryGetValue(cacheKey, out FieldInfo[] matchingFields)) {
                matchingFields = ComputeReflectedFields<T>();
                _reflectedFieldCache[cacheKey] = matchingFields;
            }

            List<T> toReturn = new List<T>(matchingFields.Length);
            foreach (FieldInfo field in matchingFields) {
                if (field.GetValue(this) is T value) {
                    toReturn.Add(value);
                }
            }

            return toReturn.ToArray();
        }

        private FieldInfo[] ComputeReflectedFields<T>() {
            // DeclaredOnly at each level of the walk: inherited public/protected fields are
            // already visible via the most-derived type's own GetFields call, so including them
            // again at every base-type level would double them up in the result.
            List<FieldInfo> matching = new List<FieldInfo>();
            Type currentType = GetType();
            do {
                foreach (FieldInfo field in currentType.GetFields(ReflectedFieldBindingFlags)) {
                    if (typeof(T).IsAssignableFrom(field.FieldType)) {
                        matching.Add(field);
                    }
                }

                currentType = currentType.BaseType;
            } while (currentType != null);

            return matching.ToArray();
        }
    }
}