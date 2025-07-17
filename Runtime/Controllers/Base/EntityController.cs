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
        protected virtual void Initialize() { }
        
        /// <summary>
        /// Finds a given set of fields via Reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T[] FindReflectedFields<T>() {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            
            List<FieldInfo> fields = new List<FieldInfo>();
            Type baseType = GetType();
            do {
                foreach(FieldInfo field in baseType.GetFields(bindingFlags)) {
                    fields.Add(field);
                }

                baseType = baseType.BaseType;
            } while(baseType != null);

            List<T> toReturn = new List<T>();
            foreach(FieldInfo field in fields) {
                if(field.FieldType.IsSubclassOf(typeof(T))){
                    toReturn.Add((T)field.GetValue(this));
                }
            }

            return toReturn.ToArray();
        }
    }
}