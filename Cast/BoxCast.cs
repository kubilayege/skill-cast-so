using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cast Types/Box Cast")]
public class BoxCast : CastBase
{
    public Vector3 halfExtents;

    public override async UniTask<(T[] instances, Vector3[] hitPositions)> Cast<T>(CastData castData, params Type[] excludedTypes)
    {
        var at = castData.AgentParent.position + castData.AgentParent.forward * halfExtents.z;
        var coneCastAllResult = BoxCastAll<T>(at, castData.AgentParent.forward, halfExtents);
        return await TaskManager.FromResult(ExcluceFrom(castData, coneCastAllResult.instances, coneCastAllResult.hitPositions, excludedTypes));
    }

    public override CastBase Clone()
    {
        CastBase caster = Instantiate(this);

        return caster;
    }

    public override float GetRange()
    {
        return halfExtents.z;
    }

    protected (T[] instances, List<Vector3> hitPositions) BoxCastAll<T>(Vector3 at, Vector3 direction, Vector3 size)
    {
        Physics.SyncTransforms();
        RaycastHit[] boxCastHits = Physics.BoxCastAll(at, size, direction, Quaternion.identity, castLayer);
        List<T> boxCastHitList = new List<T>();
        var hitPositions = new List<Vector3>();
        if (boxCastHits.Length > 0)
        {
            for (int i = 0; i < boxCastHits.Length; i++)
            {
                Vector3 hitPoint = boxCastHits[i].collider.ClosestPoint(at);
                hitPoint.y = at.y;
                if (!boxCastHits[i].transform.TryGetComponent(out T component)) continue;
                hitPositions.Add(hitPoint);
                boxCastHitList.Add(component);
            }
        }

        var coneCastHits = boxCastHitList.ToArray();

        return (coneCastHits, hitPositions);
    }
}