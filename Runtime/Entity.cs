using UnityEngine;

namespace BBUnity.Entities {

    /// <summary>
    /// The base class for all Entities. Implements IEntity.
    /// </summary>
    public class Entity : MonoBehaviour, IEntity {

        /*
         * The lifecycle functions which are called for each entity
         * Note - These are not your standard Unity calls, but are called
         * as part of those methods which are used by the base Characters
         */
        // protected virtual void OnAwake() { }
        // protected virtual void OnStart() { }
        // protected virtual void OnUpdate() { }
    }
}