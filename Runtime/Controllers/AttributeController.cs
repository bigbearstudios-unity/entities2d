
using UnityEngine;

using BBUnity.Gameplay.Attributes.Base;
using BBUnity.Entities.Controllers.Base;
using System.Linq;

namespace BBUnity.Gameplay.Attributes {

    /// <summary>
    /// Controller for all attributes. This class should be overridden
    /// and attributes added via:
    /// [SerializeField]
    /// Health _health;
    /// 
    /// The only real functionality of this controller is to call 'Reset' on the
    /// Attributes
    /// </summary>
    [DefaultExecutionOrder(10)]
    public class AttributeController : EntityController {

        Attribute[] _attributes;
        DynamicAttribute[] _dynamicAttributes;

        protected virtual void Awake() {
            _attributes = FindReflectedFields<Attribute>();
            _dynamicAttributes = FindReflectedFields<DynamicAttribute>();
        }

        protected virtual void Start() {
            foreach(Attribute attribute in _attributes) {
                attribute.Reset();
            }
        }

        protected virtual void Update() {
            float deltaTime = Time.deltaTime;

            foreach(DynamicAttribute attribute in _dynamicAttributes) {
                attribute.Update(deltaTime);
            }
        }
    }
}