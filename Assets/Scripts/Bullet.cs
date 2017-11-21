using System.Collections;
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

	void Start ()
    {
        RB.velocity = transform.forward * _speed;
        RB.useGravity = false;
	}

    private void FixedUpdate()
    {
        TrackBounce();
        // keep track of velocity for collision
        _velocity = RB.velocity.magnitude;

        // reset
        if (Input.GetKey(KeyCode.R))
            Destroy(gameObject);
    }

    // check for new bounce direction
    private void TrackBounce()
    {
        Vector3 bounceDir = GetNextBounceDir();

        if (bounceDir != new Vector3())
            _nextBounceDir = bounceDir;
    }

    private Vector3 GetNextBounceDir()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _velocity))
            return Vector3.Reflect(transform.forward, hit.normal).normalized;
        else
            return new Vector3();
    }

    private void OnCollisionEnter(Collision collision)
    {
        transform.forward = _nextBounceDir;
        RB.velocity = _nextBounceDir * _velocity;
        RB.angularVelocity = new Vector3();

        // for corners
        TrackBounce();
    }

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

    private SphereCollider _col;
    private SphereCollider COL
    {
        get
        {
            if (_col == null)
                _col = GetComponent<SphereCollider>();

            return _col;
        }
    }

}
