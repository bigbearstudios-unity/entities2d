using BBUnity;

namespace BBUnity.Entities {

    /// <summary>
    /// The base class for all Entities. This inhertits from BBMonoBehaviour
    /// and implements IEnity.
    /// </summary>
    public class Entity : BBMonoBehaviour, IEntity {

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