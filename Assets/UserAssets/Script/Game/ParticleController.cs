using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

    [PunRPC]
    public void ParticlePlay(Vector3 pos)
    {
        StartCoroutine(ParticlePlayCoroutine());
    }

    private IEnumerator ParticlePlayCoroutine()
    {
        foreach (ParticleSystem ps in mParticleSystems)
            ps.Play();

        yield return mWaitForTime;

        ParticleStop();
    }

    private void ParticleStop()
    {
        foreach(ParticleSystem ps in mParticleSystems)
            ps.Stop();
    }
}
