using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PhysicsGun : MonoBehaviour
{

	public float blastHeightModifier = 4.0f;
	public float maxPower = 6.0f;
	private float range;
	private float power;
	private int charging;
	Text chargeText;

	RigidbodyFPSController player;
	Transform aim;

	string pushAxis;
	string pullAxis;

	void Start ()
	{

		chargeText = GameObject.Find ("Charge").GetComponent<Text> ();
		player = GetComponentInParent<RigidbodyFPSController> ();
		aim = GameObject.Find ("AimDirection").GetComponent<Transform> ();

		if (GameStateManager.joysticksCount != 0) {
			pushAxis = "FireP" + transform.parent.GetComponent<PlayerControl> ().index;
			pullAxis = "AltFireP" + transform.parent.GetComponent<PlayerControl> ().index;
		} else {
			pushAxis = "FireP0";
			pullAxis = "AltFireP0";
		}
	}

	void Update ()
	{
		chargeText.text = power.ToString ();
		if (charging != 0 && Mathf.Abs (power) < maxPower) {
			power += charging * maxPower / 25;
			range += maxPower / 25;
		}

		if (GameStateManager.joysticksCount != 0 && Input.GetAxis (pushAxis) < .5f) {
			charging = 1;
		} else if (Input.GetButtonDown (pushAxis)) {
			charging = 1;
		}

		else if (GameStateManager.joysticksCount != 0 && Input.GetAxis (pullAxis) > -.5f) {
			charging = -1;
		} else if (Input.GetButtonDown (pullAxis)) {
			charging = -1;
		}

		if (Input.GetAxis (pushAxis) == 0 || Input.GetButtonUp (pushAxis) || Input.GetAxis (pullAxis) == 0 || Input.GetButtonUp (pullAxis)) {
			Collider[] hitColliders = Physics.OverlapSphere (transform.position, range * 100.0f);
			foreach (Collider c in hitColliders) {
				Rigidbody rb = c.GetComponent<Rigidbody> ();
				if (rb) {
					Vector3 dir = rb.transform.position - aim.transform.position;
					Vector3 targetDir = rb.transform.position - transform.position;
					float angle = Vector3.Angle (targetDir, transform.forward);
					if (angle < 45.0f && charging > 0) {
						rb.AddForce ((rb.transform.position - transform.position) * power, ForceMode.VelocityChange);
					} else if (transform.IsChildOf (c.transform) && c.transform.tag == "Player" && hitColliders.Length > 2) {
						if (charging > 0) {
							dir.x = Mathf.Clamp (dir.x, -0.5f, 0.5f);
							dir.z = Mathf.Clamp (dir.z, -0.5f, 0.5f);
							rb.AddForce (dir * power * blastHeightModifier, ForceMode.VelocityChange);
						} 
					} 
					else if (charging < 0) {
						
					}
				}
			}
			charging = 0;
			range = power = 0;
		}
	}
}