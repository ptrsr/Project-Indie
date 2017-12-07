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
        smallCrateClone.transform.parent = Arm.transform;
    }


    public void PickUpBox ()
    {
		if (smallCrateClone != null)
        	smallCrateClone.transform.parent = Arm.transform;
    }

    public void ThrowBox ()
    {
        Vector3 explosionPos = Box.transform.position;
		if (smallCrateClone != null)
		{
			smallCrateClone.transform.parent = null;
			smallCrateClone.GetComponent <Rigidbody> ().AddForce (transform.up * 150);
			smallCrateClone.GetComponent <Rigidbody> ().AddForce ((transform.forward + transform.up) * 300);
		}
    }

    public void ResetThrow ()
    {
        GetComponent <Animator> ().SetInteger ("ThrowBox", 0);
    }
}
