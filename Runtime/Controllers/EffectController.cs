using UnityEngine;

using System.Collections.Generic;

using BBUnity.Entities.Controllers.Base;
using BBUnity.Pools;
using UnityEngine.UIElements;

namespace BBUnity.Entities.Controllers {

    [System.Serializable]
    public class EffectReference {

        [SerializeField]
        private string _name;
        public string Name { get { return _name; } }

        [SerializeField]
        private GameObject _prefab;
        public GameObject Prefab { get { return _prefab; } }

        [SerializeField]
        private Transform _position;
        public Transform Position { get { return _position; } }
    }

    internal class EffectDictionaryReference {
        public EffectReference _effectReference;
        public ObjectPoolReference _poolReference;

        internal EffectDictionaryReference(EffectReference effectReference, ObjectPoolReference poolReference) {
            _effectReference = effectReference;
            _poolReference = poolReference;
        }
    }

    /// <summary>
    /// Controller to aid with the calling / rendering of effects
    /// </summary>
    public class EffectController : EntityController {
        
        [SerializeField]
        private List<EffectReference> _effects = new List<EffectReference>();

        private Dictionary<string, EffectDictionaryReference> _internalEffects = new Dictionary<string, EffectDictionaryReference>();

        private void Awake() {
            ObjectPool pool = ObjectPool.FindInScene("Effect Pool");
            if(pool == null) {
                pool = Utilities.Create.GameObject("Effect Pool", new [] { typeof(ObjectPool) }).GetComponent<ObjectPool>();
            }

            foreach(EffectReference effectReference in _effects) {
                ObjectPoolReference poolReference = pool.FindPoolReference(effectReference.Name);
                if(poolReference == null) {
                    poolReference = new ObjectPoolReference(effectReference.Name, effectReference.Prefab, 1, 100);
                    pool.AddPoolReference(poolReference);
                }

                _internalEffects.Add(
                    effectReference.Name,
                    new EffectDictionaryReference(effectReference, poolReference)
                );
            }
        }

        private void InstantiateEffect(string name) {
            EffectDictionaryReference reference = _internalEffects[name];

            PoolBehaviour obj = reference._poolReference.Spawn();

            obj.transform.position = reference._effectReference.Position.position;
            obj.transform.localScale = gameObject.transform.localScale;
        }
    }
}