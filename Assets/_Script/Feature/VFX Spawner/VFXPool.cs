using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

using static UnityEngine.Object;

namespace Gimersia.Feature.VFXSpawner {
    [Serializable]
    public class VFXPool {
        public ParticleSystem VFXPrefab;
        public int DefaultCapacity = 3;
        public int MaxSize = 10;
        public bool CollectionCheck = true;
        ObjectPool<ParticleSystem> _pool;
        public VFXPool()
        {
            _pool = new ObjectPool<ParticleSystem>(
                createFunc: () => Instantiate(VFXPrefab),
                actionOnGet: static b => b.gameObject.SetActive(true),
                actionOnRelease: static b => b.gameObject.SetActive(false),
                actionOnDestroy: static b => Destroy(b.gameObject),
                collectionCheck: CollectionCheck,
                defaultCapacity: DefaultCapacity,
                maxSize: MaxSize
            );
        }
        public void Spawn(Vector3 worldSpace)
        {
            var p = _pool.Get();
            p.transform.position = worldSpace;
            p.Play(true);
            BackToPool(p).Forget();
        }
        async UniTask BackToPool(ParticleSystem p)
        {
            await UniTask.WaitUntil(() => !p.isPlaying);
            _pool.Release(p);
        }
    }
}