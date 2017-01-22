using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class push : MonoBehaviour
{
	public float blastHeightModifier = 4.0f;
	public float maxPower = 10.0f;
	private float range;
	private float power;
	private int charging;
	Text chargeText;
	private Transform shape;
	RigidbodyFPSController player;
	Transform aim;

	void Start ()
	{
		chargeText = GameObject.Find ("Charge").GetComponent<Text> ();
		player = GetComponentInParent<RigidbodyFPSController> ();
		aim = GameObject.Find ("AimDirection").GetComponent<Transform> ();
	}

	void Update ()
	{
		chargeText.text = power.ToString ();
		if (charging != 0 && Mathf.Abs(power) < maxPower) {
			power += charging*maxPower/25;
			range += maxPower/25;
		}
		if (Input.GetButtonDown ("Fire1")) {
			charging = 1;
		}
		if (Input.GetButtonDown ("Fire2")) {
			charging = -1;
		}
		if (Input.GetButtonUp ("Fire1") || Input.GetButtonUp("Fire2")) {
			Collider[] hitColliders = Physics.OverlapSphere (transform.position, range * 2f);
			foreach (Collider c in hitColliders) {
				Rigidbody rb = c.GetComponent<Rigidbody> ();
				if (rb) {
					Vector3 dir = rb.transform.position - aim.transform.position;
					Vector3 targetDir = rb.transform.position - transform.position;
					float angle = Vector3.Angle (targetDir, transform.forward);
					if (angle < 45.0f) {
						if (charging > 0 || player.grounded) {
							rb.AddForce ((rb.transform.position - transform.position) * power, ForceMode.VelocityChange);
							//rb.GetComponent<MeshRenderer> ().material.SetColor ("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
						}
					}

					if ((transform.IsChildOf (c.transform) && c.transform.tag == "Player" && hitColliders.Length > 2) && ((charging > 0) || ((!player.grounded && charging < 0)))) {
						if (charging > 0) {
							dir.x = Mathf.Clamp (dir.x, -0.5f, 0.5f);
							dir.z = Mathf.Clamp (dir.z, -0.5f, 0.5f);
						} 
						rb.AddForce (dir * power * blastHeightModifier, ForceMode.VelocityChange);
					}
				}
			}
			charging = 0;
			range = power = 0;
		}
	}
}