using System.Collections.Generic;
using UnityEngine;

namespace BBUnity.Entities.Controllers.Base {

    public interface IEntityController { }

    /// <summary>
    /// The base controller for all entity controllers. 
    /// </summary>
    public class EntityController : MonoBehaviour, IEntityController {

        protected virtual void Initialize() { }
        
        protected T[] FindReflectedFields<T>() {
            System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            System.Reflection.FieldInfo[] fields = GetType().GetFields(bindingFlags);

            List<T> toReturn = new List<T>();
            foreach(System.Reflection.FieldInfo field in fields) {
                if(field.FieldType.IsSubclassOf(typeof(T))){
                    toReturn.Add((T)field.GetValue(this));
                }
            }

            return toReturn.ToArray();
        }
    }
}