using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class CastBase : ScriptableObject
{
    public LayerMask castLayer;
    public LayerMask obstacleLayer;
    public bool doLineOfSightCheck = true;

    public virtual UniTask<(T[] instances, Vector3[] hitPositions)> Cast<T>(CastData castData, params Type[] excludedTypes)
    {
        return TaskManager.FromResult<(T[] instances, Vector3[] hitPositions)>((null,null));
    }
    
    protected virtual (T[] instances, Vector3[] hitPositions) ExcluceFrom<T>(CastData castData,T[] hitList, List<Vector3> positions, params Type[] excludedTypes)
    {
        var excludedList = new List<T>(hitList);
        foreach (var hit in hitList)
        {
            foreach (var excludedType in excludedTypes)
            {
                Debug.Log(hit.GetType());
                if (hit.GetType().BaseType == excludedType)
                {
                    positions.RemoveAt(excludedList.IndexOf(hit));
                    excludedList.Remove(hit);
                }
            }
        }

        if (doLineOfSightCheck)
            return ExcludeOutOfSight(castData, excludedList, positions);

        return (excludedList.ToArray(), positions.ToArray());
    }

    protected virtual (T[] instances, Vector3[] hitPositions) ExcludeOutOfSight<T>(CastData castData, List<T> excludedList, List<Vector3> positions)
    {
        var tempList = new List<T>();
        var tempPosList = new List<Vector3>();
        int i = 0;
        foreach (var instance in excludedList)
        {
            var inBetweenVector = positions[i] - castData.AgentParent.position;
            if (!Physics.Raycast(castData.AgentParent.position + Vector3.up, inBetweenVector.normalized, inBetweenVector.magnitude, obstacleLayer))
            {
                tempList.Add(instance);
                tempPosList.Add(positions[i]);           
                i++;
            }
        }
        return (tempList.ToArray(), tempPosList.ToArray());
    }

    public abstract CastBase Clone();

    public abstract float GetRange();
}