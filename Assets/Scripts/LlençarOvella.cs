﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LlençarOvella : MonoBehaviour {

	public float minimumAngle;
	public float maxMaximumAngle;
	public float maxStartingAngularSpeed;
	public float finalAngularSpeed;
	public GameObject ovella;
	public GameObject prefabOvella;
	public GameObject pivotPoint;
	public Slider Force;
	public float maxChargeTime;
	public Transform Troll;
	public float timeToNewOvella;

	private float startTime;
	private float finalTime;
	private float time;
	private float chargetime;
	private float timeSinceLastLaunch;
	private bool attached;
	private bool waiting;
	private float maximumAngle;
	private float startingAngularSpeed;

	static float MathMod(float a, float b) {
		return (Mathf.Abs(a * b) + a) % b;
	}

	// Use this for initialization
	void Start () {
		attached = true;
		waiting = true;
		time = 1000;
		startTime = Mathf.Abs (maximumAngle - minimumAngle) / startingAngularSpeed;		
		finalTime = Mathf.Abs (maximumAngle - minimumAngle) / finalAngularSpeed;
	}

	void createOvella (){
			if (!attached) {
						if (ovella != null)
								ovella.transform.parent = transform.root;
						ovella = Instantiate (prefabOvella, transform.position + transform.rotation * prefabOvella.transform.position, transform.rotation * prefabOvella.transform.rotation) as GameObject;
						ovella.transform.parent = transform;
						attached = true;
						waiting = true;
						time = 1000;
				}
		}

	public void OvellaChoca(){
		StartCoroutine ("wait");
		}
    IEnumerator wait(){
		yield return new WaitForSeconds (timeToNewOvella);
		createOvella ();
	}

	void Update (){






		//if (ovella != null) Debug.Log (ovella.name);
		if (!attached) timeSinceLastLaunch += Time.deltaTime;
		if (ovella == null || (!attached && (ovella.rigidbody2D.velocity.magnitude == 0))) {
			createOvella();
		}
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
						chargetime = 0;

		}
		if (Input.GetKey (KeyCode.Mouse0)){
			chargetime+=Time.deltaTime;
			chargetime=Mathf.Clamp (chargetime,0,maxChargeTime);
			Force.value=(chargetime/maxChargeTime)*(Force.maxValue-Force.minValue)+Force.minValue;
		}
		if (Input.GetKeyUp(KeyCode.Mouse0)){
			startingAngularSpeed=maxStartingAngularSpeed*(Force.value-Force.minValue)/(Force.maxValue-Force.minValue);
			Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pz.z=0;
			Vector2 OP = pz - pivotPoint.transform.position;
			float R = (pivotPoint.transform.position - ovella.transform.position).magnitude;
			if(OP.magnitude<=R){
				maximumAngle = -60;
			}else{
				float theta = Mathf.Acos (R / OP.magnitude);
				float alpha = Mathf.Atan2 (OP.y, OP.x);
				float beta = (theta + alpha)*180/Mathf.PI;
				maximumAngle=beta-180;
			}
			Debug.Log (maximumAngle);
			startTime = Mathf.Abs (maximumAngle - minimumAngle) / startingAngularSpeed;		
			finalTime = Mathf.Abs (maximumAngle - minimumAngle) / finalAngularSpeed;
			time=0;
			waiting=false;
			timeSinceLastLaunch=0;
		}
	}
	// Update is called once per frame
	void FixedUpdate () {	
		time += Time.fixedDeltaTime;
		float angle=minimumAngle;
		if (time < startTime)
						angle = minimumAngle - time * startingAngularSpeed;
		else {
			if (attached & !waiting){
				attached=false;
				ovella.rigidbody2D.isKinematic=false;
				Vector2 OP= (ovella.rigidbody2D.position-pivotPoint.rigidbody2D.position)*startingAngularSpeed*Mathf.PI/180;
				ovella.rigidbody2D.velocity = new Vector2(OP.y,-OP.x);
				ovella.rigidbody2D.angularVelocity = -10;
			}
			if (time < startTime + finalTime)angle = maximumAngle + (time - startTime) * finalAngularSpeed;
		}
		gameObject.rigidbody2D.MoveRotation(angle);
	}
}
