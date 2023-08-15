using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private float mParticleTime = 1;

    private ParticleSystem[] mParticleSystems;
    private WaitForSeconds mWaitForTime;

    private void Awake()
    {
        mParticleSystems = GetComponentsInChildren<ParticleSystem>();
        mWaitForTime = new WaitForSeconds(mParticleTime);
    }

    public IEnumerator ParticlePlayCoroutine(Vector3 pos)
    {
        transform.position = pos;
        foreach (ParticleSystem ps in mParticleSystems)
            ps.Play();

        yield return mWaitForTime;

        foreach (ParticleSystem ps in mParticleSystems)
            ps.Stop();
    }
}
