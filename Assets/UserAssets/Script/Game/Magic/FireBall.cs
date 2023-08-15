using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireBall : Magic
{
    [SerializeField] private LayerMask mAttackableLayer;
    [SerializeField] private LayerMask mGroundLayer;
    [SerializeField] private float explosionRange;

    private PhotonView mPhotonView;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    
    private float speed = 12;
    private int damage;

    private int mTempDamage = 50;

    private Vector3 dir;
    private void Awake()
    {
        mPhotonView = GetComponent<PhotonView>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public override void Init(Vector3 destinationPos)
    {
        base.Init(destinationPos);

        mPhotonView.RPC(nameof(InitRPC),RpcTarget.All);
        dir = (destinationPos - transform.position).normalized;
    }

    [PunRPC]
    private void InitRPC()
    {
        isBatch = true;
    }

    private void Update()
    {
        if (!isBatch) return;
        Throw();
    }

    private void Throw()
    {
        transform.position += speed * Time.deltaTime * dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isBatch) return;
        if (1 << other.gameObject.layer != mGroundLayer.value) return;
        Debug.Log("Trigger Enter");
        Explosion();
        Explosion(other);
    }

    public void Explosion(Collider other)
    {
        isBatch = false;
        meshRenderer.enabled = false;
        PhotonNetwork.Destroy(mPhotonView);
    }
    
    public void Explosion()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, explosionRange, mAttackableLayer);
        for(int i = 0; i < col.Length; i++)
        {
            if (col[i].TryGetComponent(out Damageable damageable))
            {
                //damageable.GetDamage(mTempDamage);

                Debug.Log("Explosion");
            }
        }
    }
}
