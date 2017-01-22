using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube : MonoBehaviour {
	Rigidbody rb;

	void Start(){
	//	rb = GetComponent<Rigidbody> ();
	}

	void OnCollisionEnter(Collision col) {
	//	if (col.transform.tag == "Player") {
		//	col.transform.GetComponent<Rigidbody> ().AddForce (rb.velocity);
		//	Debug.Log (rb.velocity);
	//	}
	}
		
}
