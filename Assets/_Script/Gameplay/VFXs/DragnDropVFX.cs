using System;
using Gimersia.Feature.VFXSpawner;
using Oneiric.Core.EventBus;
using UnityEngine;

namespace Gimersia.Gameplay
{
    public class DragnDropVFX : MonoBehaviour
    {
        public VFXPool EntityDropVFX;
        IDisposable[] _subs;
        void Awake()
        {
            _subs = new IDisposable[] {
                EventBus<EntityDropEvent>.Bind(evt => {
                    EntityDropVFX.Spawn(evt.WorldPosition);
                })
            };
        }
        void OnDestroy() {
            foreach (var sub in _subs) sub.Dispose();
        }
    }
}
