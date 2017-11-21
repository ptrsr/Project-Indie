using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float
        _speed;

    private float
        _velocity;

    private Vector3
        _nextBounceDir;

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
        UpdateBounce();
        // keep track of velocity for collision
        _velocity = RB.velocity.magnitude;

        // reset
        if (Input.GetKey(KeyCode.R))
            Destroy(gameObject);
    }

    // check for new bounce direction
    private void UpdateBounce()
    {
        #if UNITY_EDITOR
        _bounces[_bounces.Count - 1] = transform.position;
        #endif

        Vector3 bounceDir = GetNextBounceDir();

        if (bounceDir != new Vector3())
            _nextBounceDir = bounceDir;
    }

    private Vector3 GetNextBounceDir()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _velocity))
        {
            Vector3 reflection = Vector3.Reflect(transform.forward, hit.normal).normalized;
            reflection.y = 0; // bullet never moves on Y direction

            return reflection.normalized;
        }
        else
            return new Vector3();
    }

    private void OnCollisionEnter(Collision collision)
    {
        transform.forward = _nextBounceDir;
        RB.velocity = _nextBounceDir * _velocity;
        RB.angularVelocity = new Vector3();

        // for corners
        UpdateBounce();

        #if UNITY_EDITOR
        // track bounce
        _bounces.Add(transform.position);
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
