using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBreaker : MonoBehaviour {

	[Header ("Prefabs")]
    [SerializeField] private GameObject bCrate;

	[SerializeField] private GameObject [] pickUps;

	private bool hasCrashed = false;

	void Start ()
	{
		StartCoroutine (FailSafe ());
	}

	IEnumerator FailSafe ()
	{
		yield return new WaitForSeconds (1.34f);
		transform.parent = null;
	}

    private void OnCollisionEnter (Collision col)
    {
		if (hasCrashed)
			return;
		
        if (col.gameObject.tag == "Floor")
        {
			hasCrashed = true;
            Vector3 OldPos = transform.position;
            Quaternion OldRot = transform.rotation;
            Destroy(gameObject);
            GameObject bCrateClone = Instantiate(bCrate, OldPos, OldRot);

            Rigidbody body = GetComponent<Rigidbody>();

            for (int i = 0; i < bCrateClone.transform.childCount; i++)
            {
                Rigidbody childBody = bCrateClone.transform.GetChild(i).GetComponent<Rigidbody>();
                childBody.velocity = body.velocity;
            }
            Destroy(bCrateClone, 3f);
			GameObject PowerUpClone = Instantiate (pickUps [Random.Range (0, pickUps.Length)], transform.position, transform.rotation, transform.parent);
            PowerUpClone.GetComponent<PowerUp>().corPos = bCrateClone.transform.position;
        }
    }

}
