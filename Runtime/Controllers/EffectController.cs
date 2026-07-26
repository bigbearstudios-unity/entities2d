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
            if (pool == null) {
                Debug.Log($"EffectController on '{gameObject.name}' found no existing \"Effect Pool\" in the scene; creating one.", this);
                pool = Utilities.Create.GameObject("Effect Pool", components: new[] { typeof(ObjectPool) }).GetComponent<ObjectPool>();
            }

            foreach (EffectReference effectReference in _effects) {
                if (effectReference.Name == null) {
                    Debug.LogError($"EffectController on '{gameObject.name}' has an EffectReference with no name set; it will be skipped.", this);
                    continue;
                }

                if (effectReference.Prefab == null) {
                    Debug.LogError($"EffectController on '{gameObject.name}' has no prefab assigned for effect '{effectReference.Name}'; it will be skipped.", this);
                    continue;
                }

                ObjectPoolReference poolReference = pool.FindPoolReference(effectReference.Name);
                if (poolReference == null) {
                    poolReference = new ObjectPoolReference(effectReference.Name, effectReference.Prefab, 1, 100);
                    pool.AddPoolReference(poolReference);
                }

                _internalEffects.Add(
                    effectReference.Name,
                    new EffectDictionaryReference(effectReference, poolReference)
                );
            }
        }

        public void InstantiateEffect(string name) {
            if (!_internalEffects.TryGetValue(name, out EffectDictionaryReference reference)) {
                Debug.LogError($"EffectController on '{gameObject.name}' has no registered effect named '{name}'.", this);
                return;
            }

            PoolBehaviour obj = reference._poolReference.Spawn();

            obj.transform.position = reference._effectReference.Position.position;
            obj.transform.localScale = gameObject.transform.localScale;
        }

        public void InstantiateEffect(string name, Vector3 position) {
            if (!_internalEffects.TryGetValue(name, out EffectDictionaryReference reference)) {
                Debug.LogError($"EffectController on '{gameObject.name}' has no registered effect named '{name}'.", this);
                return;
            }

            PoolBehaviour obj = reference._poolReference.Spawn();
            obj.transform.localScale = gameObject.transform.localScale;
            obj.transform.position = position;
        }
    }
}