using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    [SerializeField] private Transform neutralUnitSpawnPos;
    [SerializeField] private Transform neutralUnitInitPos;
    [SerializeField] private NeutralUnit neutralUnit;


    [ContextMenu("SpawnUnit")]
    public void Spawn()
    {
        Instantiate(neutralUnit, neutralUnitSpawnPos.position, Quaternion.LookRotation(Vector3.back)).Init(neutralUnitInitPos.position);
    }
}
