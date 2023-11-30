using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cast Types/Projectile Cast")]
public class ProjectileCast : CastBase
{
    [SerializeField] private GameObjectPoolItem projectile;
    [SerializeField] private float range;
    
    public override async UniTask<(T[] instances, Vector3[] hitPositions)> Cast<T>(CastData castData, params Type[] excludedTypes)
    {
        var projectileInstance = GameObjectPool.Spawn<Projectile>(projectile, castData.GetCastPoint(), Quaternion.LookRotation(castData.AgentParent.forward));
        var launchTask = projectileInstance.Launch<T>(excludedTypes, castData);
        await launchTask;

        return launchTask.Result;
    }

    public override CastBase Clone()
    {
        CastBase caster = Instantiate(this);

        return caster;
    }

    public override float GetRange()
    {
        return range;
    }
}