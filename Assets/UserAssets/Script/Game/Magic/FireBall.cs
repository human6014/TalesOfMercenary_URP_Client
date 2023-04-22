using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Magic
{
    [SerializeField] private LayerMask explosionLayer;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private float explosionRange;
    private float speed = 1;
    private int damage;

    private Vector3 dir;
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }
    public override void Init(Vector3 destinationPos)
    {
        base.Init(destinationPos);

        isBatch = true;
        dir = destinationPos - transform.position;
    }

    private void FixedUpdate()
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
        if (1 << other.gameObject.layer != explosionLayer.value) return;
        Explosion(other);
    }

    public void Explosion(Collider other)
    {
        Debug.Log("Explosion");
        isBatch = false;
        meshRenderer.enabled = false;

    }
    /*
    public void Explosion()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, explosionRange, explosionLayer);
        for(int i = 0; i < col.Length; i++)
        {
            if (col[i].TryGetComponent(out Unit unit))
            {
                unit.Hit(damage);
                Debug.Log("Explosion");
            }
        }
    }
    */
}
