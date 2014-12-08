﻿using UnityEngine;
using System.Collections;


// panning properties

[System.Serializable]
public class Panning {
	public Vector3 position = new Vector3(0, 0.35f, 0);
	public float speed = 1f;
	public float interpolation = 5f;
	public bool enabled = false;
}


// rotating properties

[System.Serializable]
public class Rotating {
	public Vector3 angle = new Vector3(55, 30, 0);
	public float speed = 25f;
	public float interpolation = 25f;
	public bool enabled = false;

	public float xAngleMax = 90f;
	public float xAngleMin = 0f;
}


// zooming properties

[System.Serializable]
public class Zooming {
	[Range(1f, 120f)]
	public float distance = 60f;
	public float speed = 15f;
	public float interpolation = 5f;
	[HideInInspector]
	public float distanceMin = 3f;
	[HideInInspector]
	public float distanceMax = 120f;
}


// camera class

public class CameraControls : MonoBehaviour {

	public Transform target;
	//public float interval = 5.0f; // bigger numbers increase interpolation speed
	public Panning panning;
	public Rotating rotating;
	public Zooming zooming;


	void Start () {
		// warn if there is no target
		if(!target) {
			Debug.LogError("A camera target is required!");
			return;
		}

		// initialize position
		//get new pos and rot from increments
		transform.rotation = Quaternion.Euler(rotating.angle.x, rotating.angle.y, 0);
		transform.position =  transform.rotation * new Vector3(0.0f, 0.0f, -zooming.distance) + target.position + panning.position;
	}


	// manage user input on update

	void Update () {
		// middle button to pan
		if (Input.GetMouseButtonDown(2)) panning.enabled = true;
		if (Input.GetMouseButtonUp(2)) panning.enabled = false;
		if (panning.enabled) {
			float pspeed = panning.speed; // * zooming.distance * 0.025f;
			Vector3 direction = Camera.main.transform.TransformDirection(new Vector3(
				Input.GetAxis("Mouse X") * pspeed, 
				Input.GetAxis("Mouse Y") * pspeed,
				Input.GetAxis("Mouse Y") * pspeed
			));
			panning.position.x -= direction.x;
			//panning.position.y -= direction.z;
			panning.position.z -= direction.z;
		}

		// right button to rotate
		if (Input.GetMouseButtonDown(1)) rotating.enabled = true;
		if (Input.GetMouseButtonUp(1)) rotating.enabled = false;
		if (rotating.enabled) {
			rotating.angle.y += Input.GetAxis("Mouse X") * rotating.speed;
			rotating.angle.x -= Input.GetAxis("Mouse Y") * rotating.speed;
			rotating.angle.x = ClampAngle(rotating.angle.x, rotating.xAngleMin, rotating.xAngleMax);
		}

		// mouse wheel to zoom
		zooming.distance = Mathf.Clamp(
			zooming.distance - Input.GetAxis("Mouse ScrollWheel") * zooming.speed, 
			zooming.distanceMin, 
			zooming.distanceMax
		);
	}


	// update position and rotation on late update

	void LateUpdate () {
		if (!target) return;

		//get new pos and rot from increments
		Quaternion rotation = Quaternion.Euler(rotating.angle.x, rotating.angle.y, 0);
		Vector3 position =  rotation * new Vector3(0.0f, 0.0f, -zooming.distance) + target.position + panning.position;

		//interpolate camera to new pos and rotation
		//float interval = this.interval;
		//if (rotating) interval = 25;

		//float interval = 5;
		/*if (rotating.enabled) interval = rotating.interpolation;
		if (panning.enabled) interval = rotating.panning;
		if (zooming.enabled) interval = rotating.zooming;*/
		float interval = rotating.enabled ? rotating.interpolation : panning.interpolation;

		float time = Time.deltaTime;
		transform.position = Vector3.Slerp(transform.position, position, time * interval);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, time * 25);
	}


	// utility functions

	float ClampAngle(float angle , float min , float max ) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}


	//**********************************
	// Controls for device
	// *********************************

	/*private void setInputControls () {

		#if UNITY_EDITOR
			if (!mouseControlsEnabled) return;

			// -------------------------
			// control camera with mouse
			// -------------------------

			if (Input.GetButtonDown("Fire2")) rotating = true;
			if (Input.GetButtonUp("Fire2")) rotating = false;

			setDistance(Input.GetAxis("Mouse ScrollWheel"));

			if (rotating) {
				setRotation(
					Input.GetAxis("Mouse X"),
					Input.GetAxis("Mouse Y") * 0.5f
				);
			}

		#else
			if (!touchControlsEnabled) return;

			// --------------------------------------------------
			// control camera by pinching/rotating with 2 fingers
			// --------------------------------------------------

			if (Input.touchCount == 2) {
				rotating = true;

				// Zoom the camera while pinching with 2 fingers

				if (Input.GetTouch(1).phase == TouchPhase.Began) {
					pinchLength = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
				}

				if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved) {
					deltaLength = (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) - pinchLength) * 0.01f;
					pinchLength = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

					// set camera distance
					setDistance(deltaLength);
				}

				// Rotate camera by moving with 2 fingers

				for (int i = 0; i < Input.touchCount; ++i) {
					Touch touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began) rotating = true;
					if (touch.phase == TouchPhase.Ended) rotating = false;

					if (rotating) {
						setRotation(
							touch.deltaPosition.x * 0.05f,
							touch.deltaPosition.y * 0.05f
						);
					}
				}

			} else if (Input.touchCount == 2) {
				rotating = false;
			}
		#endif
	}*/

}


