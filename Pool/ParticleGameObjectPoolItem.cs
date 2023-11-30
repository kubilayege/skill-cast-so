using Cysharp.Threading.Tasks;
using UnityEngine;

public class ParticleGameObjectPoolItem : GameObjectPoolItem
{
    [SerializeField] private ParticleSystem Particle;

    private async void OnEnable()
    {
        var t = Particle.main.duration;
        await TaskManager.Delay(Mathf.RoundToInt(t * 1000));
        if (this == null) return;
        GameObjectPool.Despawn(this);
    }
}