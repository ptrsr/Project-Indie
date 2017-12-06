using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField]
    private GameObject _particleEffect;

    [SerializeField]
    private float _startSpeed = 20;

	[HideInInspector] public float _speed;

    private Vector3
        _velocity;

    #if UNITY_EDITOR
    private List<Vector3> _bounces;
    #endif

    void Start ()
    {
		_speed = _startSpeed;
		
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

	void Update ()
	{
		if (_speed > 100)
			_speed = 100;

		if (_speed > _startSpeed)
			_speed -= Time.deltaTime * (_startSpeed / _speed) * 4;

        #if UNITY_EDITOR
        _bounces[_bounces.Count - 1] = transform.position;
        #endif
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.transform.GetComponent<PlayerController>();

        if (player != null)
        {
            if (player.CanParry(transform.position))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0.0f, player.aim.rotation.eulerAngles.y, 0.0f));
                player.ReflectBullet(_speed);

                GameObject particle = Instantiate(_particleEffect);
                particle.transform.position = transform.position;
                particle.transform.forward = -transform.forward;
            }
            else
            {
                player.Die();
                Destroy(gameObject);
                return;
            }
        }

        #if UNITY_EDITOR
        // track bounce
        _bounces.Add(transform.position);
        #endif

        StartCoroutine(bounce());
    }

    IEnumerator bounce()
    {
        yield return new WaitForEndOfFrame();

        RB.angularVelocity = new Vector3();

        print(_speed);
        RB.velocity = RB.velocity.normalized * _speed;

        transform.forward = RB.velocity.normalized;

        GameObject particle = Instantiate(_particleEffect);
        particle.transform.position = transform.position;
        particle.transform.forward = -transform.forward;
    }

    #if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        BulletTracker tracker = ServiceLocator.Locate<BulletTracker>();

        if (tracker != null)
            tracker.AddTrajectory(_bounces);
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
