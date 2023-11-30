using UnityEngine;

public class CastData
{
    public Transform AgentParent { get; private set; }
    public Transform AgentHandLeftParent { get; private set; }
    public Transform AgentHandRightParent { get; private set; }
    public Transform AgentWeaponParent { get; private set; }

    public Vector3 CastPointOffset { get; private set; }
    
    public CastData(Transform agentParent = null, Transform agentWeaponParent = null, Transform agentHandLeftParent = null, Transform agentHandRightParent = null)
    {
        AgentParent = agentParent;
        AgentHandLeftParent = agentHandLeftParent;
        AgentHandRightParent = agentHandRightParent;
        AgentWeaponParent = agentWeaponParent;
    }

    public void ChangeReferences(
        Transform agentParent = null, 
        Transform weaponParent = null, 
        Transform leftParent = null, 
        Transform rightParent = null)
    {
        if (agentParent) AgentParent = agentParent;
        if (weaponParent) AgentWeaponParent = weaponParent;
        if (leftParent) AgentHandLeftParent = leftParent;
        if (rightParent) AgentHandRightParent = rightParent;
    }

    public float GetBaseProjectileHeight()
    {
        return Mathf.Abs((AgentParent.position - AgentWeaponParent.position).y);
    }

    public Transform GetHandParent(HandType type, bool getReversed = false)
    {
        var typeToCheck = getReversed ? HandType.Left : HandType.Right;
        return (type == typeToCheck) ? AgentHandRightParent : AgentHandLeftParent;
    }

    public Vector3 GetCastPoint()
    {
        return AgentParent.position + CastPointOffset.x * AgentParent.right + CastPointOffset.y * AgentParent.up + CastPointOffset.z * AgentParent.forward;
    }

    public void SetCastPointOffset(Vector3 dataCastPointOffset)
    {
        CastPointOffset = dataCastPointOffset;
    }
}