using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhysicsGun2 : MonoBehaviour {

	public float maxDistance = 50;
	public float maxPower = 10;
	public int chargeFrames = 30;
	private int charging = 0;
	private Camera cam;
	private float power = 0f;
	private float distance = 0f;
	private float[] rayGridY = new float[] {0.60f, 0.59f, 0.58f, 0.57f, 0.56f, 0.55f, 0.54f, 0.53f, 0.52f, 0.51f, 0.50f, 0.49f, 0.48f, 0.47f, 0.46f, 0.45f, 0.44f, 0.43f, 0.42f, 0.41f, 0.40f};
	private float[] rayGridX = new float[] {0.43f, 0.45f, 0.47f, 0.48f, 0.49f, 0.50f, 0.51f, 0.52f, 0.53f, 0.55f, 0.57f};

	string pushAxis;
	string pullAxis;

	void Start ()
	{
		cam = GetComponent<Camera> ();

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
		//DebugVision ();
		if (charging == 0) {
			if (GameStateManager.joysticksCount != 0 && Input.GetAxis (pushAxis) < .5f) {
				charging = 1;
			} else if (Input.GetButtonDown (pushAxis)) {
				charging = 1;
			} else if (GameStateManager.joysticksCount != 0 && Input.GetAxis (pullAxis) > -.5f) {
				charging = -1;
			} else if (Input.GetButtonDown (pullAxis)) {
				charging = -1;
			}
		} else if(Mathf.Abs (power) < maxPower){
			power += charging * maxPower / chargeFrames;
			distance += maxDistance / chargeFrames;
		}
		else if ((Input.GetAxis (pushAxis) == 0 || Input.GetButtonUp (pushAxis) || Input.GetAxis (pullAxis) == 0 || Input.GetButtonUp (pullAxis)) && charging != 0) {
			//Debug.Log ("Go");
			VisionCheck();
				charging = 0;
				power = 0;
		}
		else {

		}
	}

	void VisionCheck(){
		if (charging > 0) {
			List<Rigidbody> bodies = new List<Rigidbody> ();
			List<Vector3> directions = new List<Vector3> ();
			foreach (float y in rayGridY) {
				foreach (float x in rayGridX) {
					Ray ray = cam.ViewportPointToRay (new Vector3 (x, y, 0));

					RaycastHit hit;
					if (Physics.Raycast (ray, out hit) && hit.distance < distance) {
						Rigidbody rb = hit.collider.GetComponent<Rigidbody> ();
						if (rb && !bodies.Contains (rb)) {
							bodies.Add (rb);
							directions.Add (ray.direction);

						}
					}
				}
			}
			int i = 0;
			foreach (Rigidbody rb in bodies) {
				float hitDist = Vector3.Distance (rb.transform.position, transform.position);
				rb.AddForce (6 * directions [i] * power / hitDist, ForceMode.VelocityChange);
				if (hitDist < 10) {
					Rigidbody p = GetComponentInParent<Rigidbody> ();
					p.AddForce (directions [i] * -1 * power / (hitDist * 2), ForceMode.VelocityChange);
				}
			}
		} else {
			Ray ray = cam.ViewportPointToRay (new Vector3 (.5f, .5f, 0));
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit) && hit.distance < (distance*distance)) {
				Rigidbody p = GetComponentInParent<Rigidbody> ();
				p.AddForce (ray.direction * power * -1 / 20, ForceMode.VelocityChange);
			}
		}

	}

	void DebugVision() {
		foreach (float y in rayGridY) {
			foreach (float x in rayGridX) {
				Ray ray = cam.ViewportPointToRay (new Vector3 (x, y, 0));
				Debug.DrawRay (ray.origin, ray.direction * distance);
					}
				}
			}

}
