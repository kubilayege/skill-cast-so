using System;
using UnityEngine;

public class GameObjectPoolItem : MonoBehaviour
{
    public string ID = Guid.NewGuid().ToString();
    [HideInInspector] public Transform PoolParent;
}