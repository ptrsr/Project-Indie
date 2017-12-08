using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxThrowing : MonoBehaviour {

    [SerializeField] private GameObject Arm, Box;
    GameObject smallCrateClone;

	[HideInInspector] public Transform currentFollow;
    
    public void ThrowPickup ()
    {
        GetComponent <Animator> ().SetInteger ("ThrowBox", 1);
    }

    public void SpawnCrate ()
    {
		smallCrateClone = Instantiate (Box, transform.position, transform.rotation);
    }

    public void PickUpBox ()
    {/*
		if (smallCrateClone == null)
			ResetThrow ();
		else
        	smallCrateClone.transform.parent = Arm.transform;*/
    }

    public void ThrowBox ()
    {
		if (smallCrateClone == null)
		{
			ResetThrow ();
			return;
		}
		
        Vector3 explosionPos = Box.transform.position;

		smallCrateClone.transform.parent = null;
		smallCrateClone.GetComponent <BoxCollider> ().enabled = true;
		smallCrateClone.GetComponent <Rigidbody> ().useGravity = true;
		smallCrateClone.GetComponent <Rigidbody> ().AddForce (transform.up * 150);
		smallCrateClone.GetComponent <Rigidbody> ().AddForce ((transform.forward + transform.up) * 300);
    }

    public void ResetThrow ()
    {
        GetComponent <Animator> ().SetInteger ("ThrowBox", 0);
    }
}
