using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cast Types/Rain of Arrows Cast")]
public class RainOfArrowsCast : SphereCast
{
    [Header("Rain of Arrows Properties")]
    [SerializeField] private ParticleGameObjectPoolItem LaunchFX;
    [SerializeField] private ParticleGameObjectPoolItem RainOfArrowsFX;
    [SerializeField] private float ForwardAmount = 5f;
    [SerializeField] private float SpawnHeight = 10f;

    public override async UniTask<(T[] instances, Vector3[] hitPositions)> Cast<T>(CastData castData, params Type[] excludedTypes)
    {
        var launchFX = GameObjectPool.Spawn(LaunchFX, castData.AgentWeaponParent.position, castData.AgentWeaponParent.rotation).gameObject;

        var agentPosition = castData.AgentParent.position;
        var agentForward = castData.AgentParent.forward;
        
        while (launchFX.activeSelf) await UniTask.Yield();

        var rainOfArrowsImpactPosition = agentPosition + agentForward * ForwardAmount;
        var rainOfArrowsSpawnPosition = rainOfArrowsImpactPosition + Vector3.up * SpawnHeight;

        var rainOfArrowsFX = GameObjectPool.Spawn<ParticleSystem>(RainOfArrowsFX, rainOfArrowsSpawnPosition, Quaternion.identity);
        rainOfArrowsFX.transform.localScale = radius * Vector3.one;

        var rainOfArrowsHeightDistance = Vector3.Distance(rainOfArrowsImpactPosition, rainOfArrowsSpawnPosition);
        var rainOfArrowsFallSpeed = Mathf.Abs(rainOfArrowsFX.velocityOverLifetime.y.constant);
        var rainOfArrowsFallTime = rainOfArrowsHeightDistance / rainOfArrowsFallSpeed;

        await TaskManager.Delay((int) (rainOfArrowsFallTime * 1000f), true);
        
        var coneCastResult = ConeCastAll<T>(rainOfArrowsImpactPosition, Vector3.up);
        
        return await TaskManager.FromResult(ExcluceFrom(castData, coneCastResult.instances, coneCastResult.hitPositions, excludedTypes));
    }

    public override float GetRange()
    {
        return ForwardAmount;
    }
}