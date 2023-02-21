using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolableScript : MonoBehaviour
{
    abstract public void ReturnObject();
}
