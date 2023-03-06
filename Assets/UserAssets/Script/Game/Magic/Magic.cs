using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    protected bool isBatch;
    protected bool isPlayer;
    public virtual void Init(Vector3 destinationPos, bool isPlayer)
    {
        this.isPlayer = isPlayer;
    }
}
