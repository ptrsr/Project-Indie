using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float
        _speed;

    private Vector3
        _velocity;

    private List<Collision> _collisions;

    #if UNITY_EDITOR
    private List<Vector3> _bounces;
    #endif

    void Start ()
    {
        RB.velocity = transform.forward * _speed;
        RB.useGravity = false;
        RB.constraints = RigidbodyConstraints.FreezePositionY;

        _collisions = new List<Collision>();

        #if UNITY_EDITOR
        // track spawn position
        _bounces = new List<Vector3>();
        _bounces.Add(transform.position);
        _bounces.Add(transform.position);
        #endif
    }

    private void FixedUpdate()
    {
        ResolveCollisions();

        // keep track of velocity for collision
        _velocity = RB.velocity;

        #if UNITY_EDITOR
        _bounces[_bounces.Count - 1] = transform.position;
        #endif

        // reset
        if (Input.GetKey(KeyCode.R))
            Destroy(gameObject);
    }

    private void ResolveCollisions()
    {
        if (_collisions.Count == 0)
            return;

        for (int i = 0; i < _collisions.Count; i++)
            for (int j = _collisions.Count - 1; j > i; j--)
                if (_collisions[i].contacts[0].normal == _collisions[j].contacts[0].normal)
                    _collisions.RemoveAt(j);

        Vector3 normal = new Vector3();

        for (int i = 0; i < _collisions.Count; i++)
        {
            Collision collision = _collisions[i];
            ContactPoint point = collision.contacts[0];

            Vector3 newNormal = point.normal;
            newNormal.y = 0;
            newNormal.Normalize();

            normal += newNormal;
        }
        normal.Normalize();

        Vector3 nextBounceDir = Vector3.Reflect(_velocity.normalized, normal).normalized;

        transform.forward = nextBounceDir;
        RB.velocity = nextBounceDir * _velocity.magnitude;
        _velocity = RB.velocity;

        RB.angularVelocity = new Vector3();

        #if UNITY_EDITOR
        // track bounce
        _bounces.Add(transform.position);
        #endif

        _collisions.Clear();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _collisions.Add(collision);
    }

    #if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        ServiceLocator.Locate<BulletTracker>().AddTrajectory(_bounces);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        for (int i = 0; i < _bounces.Count - 1; i++)
            Gizmos.DrawLine(_bounces[i], _bounces[i + 1]);
    }
    #endif


    private Rigidbody _rb;
    private Rigidbody RB
    {
        get
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody>();

            return _rb;
        }
    }
}
