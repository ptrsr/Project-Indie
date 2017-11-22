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

    #if UNITY_EDITOR
    private List<Vector3> _bounces;
    #endif

    void Start ()
    {
        RB.velocity = transform.forward * _speed;
        RB.useGravity = false;
        RB.constraints = RigidbodyConstraints.FreezePositionY;

        #if UNITY_EDITOR 
        // track spawn position
        _bounces = new List<Vector3>();
        _bounces.Add(transform.position);
        _bounces.Add(transform.position);
        #endif
    }

    private void FixedUpdate()
    {
        // keep track of velocity for collision
        _velocity = RB.velocity;

        #if UNITY_EDITOR
        _bounces[_bounces.Count - 1] = transform.position;
        #endif

        // reset
        if (Input.GetKey(KeyCode.R))
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float seperation = collision.contacts[0].separation;

        if (seperation != 0)
        {
            Collider other = collision.collider;

            float distance = Vector3.Distance(other.ClosestPoint(transform.position), transform.position) - 0.5f;
            float dot = Vector3.Dot(-_velocity.normalized, collision.contacts[0].normal);

            if (dot != 0)
                transform.position += _velocity.normalized * distance * (1 / dot);
        }

        Vector3 nextBounceDir = Vector3.Reflect(_velocity.normalized, collision.contacts[0].normal).normalized;
        transform.forward = nextBounceDir;
        RB.velocity = nextBounceDir * _velocity.magnitude;
        _velocity = RB.velocity;

        RB.angularVelocity = new Vector3();

        #if UNITY_EDITOR
        // track bounce
        _bounces.Add(collision.contacts[0].point);
        #endif
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
