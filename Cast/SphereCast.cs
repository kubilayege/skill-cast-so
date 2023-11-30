using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cast Types/Sphere Cast")]
public class SphereCast : CastBase
{
    public float radius;
    public float distance;
    public float coneAngle = 360;

    public override async UniTask<(T[] instances, Vector3[] hitPositions)> Cast<T>(CastData castData, params Type[] excludedTypes)
    {
        if (castData.AgentParent)
        {
            var coneCastAllResult = ConeCastAll<T>(castData.AgentParent.position, castData.AgentParent.forward);
            return await TaskManager.FromResult(ExcluceFrom(castData, coneCastAllResult.instances, coneCastAllResult.hitPositions, excludedTypes));
        }
        
        return (new T[] { }, new Vector3[] { });
    }

    public override CastBase Clone()
    {
        CastBase caster = Instantiate(this);

        return caster;
    }

    public override float GetRange()
    {
        return distance;
    }

    protected (T[] instances, List<Vector3> hitPositions) ConeCastAll<T>(Vector3 at, Vector3 direction)
    {
        Physics.SyncTransforms();
        RaycastHit[] sphereCastHits = Physics.SphereCastAll(at, radius, direction, distance, castLayer);
        List<T> coneCastHitList = new List<T>();
        var hitPositions = new List<Vector3>();
        if (sphereCastHits.Length > 0)
        {
            for (int i = 0; i < sphereCastHits.Length; i++)
            {
                Vector3 hitPoint = sphereCastHits[i].collider.ClosestPoint(at);
                hitPoint.y = at.y;
                Vector3 directionToHit = hitPoint - at;
                float angleToHit = Vector3.Angle(direction, directionToHit);
                if (!(angleToHit < coneAngle)) continue;
                if (!sphereCastHits[i].transform.TryGetComponent(out T component)) continue;
                hitPositions.Add(hitPoint);
                coneCastHitList.Add(component);
            }
        }

        var coneCastHits = coneCastHitList.ToArray();

        return (coneCastHits, hitPositions);
    }
}
