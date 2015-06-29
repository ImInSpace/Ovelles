﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LlencarOvella : MonoBehaviour {

	public float minimumAngle;
	public float maxMaximumAngle;
	public float maxStartingAngularSpeed;
	public float finalAngularSpeed;
	public GameObject ovella;
	public GameObject prefabOvella;
	public Slider Force;
	public float maxChargeTime;
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
	private Vector2 Objective;

	static float MathMod(float a, float b) {
		return (Mathf.Abs(a * b) + a) % b;
	}

	// Use this for initialization
	void Start () {
		Input.simulateMouseWithTouches=true;
		attached = true;
		waiting = true;
		time = 1000;
		startTime = Mathf.Abs (maximumAngle - minimumAngle) / startingAngularSpeed;		
		finalTime = Mathf.Abs (maximumAngle - minimumAngle) / finalAngularSpeed;
	}

	void createOvella (){
			if (!attached) {
						
								ovella = Instantiate (prefabOvella, transform.position + transform.rotation * prefabOvella.transform.position, transform.rotation * prefabOvella.transform.rotation) as GameObject;
								ovella.transform.parent = transform;
								attached = true;
								if (time>=startTime)waiting = true;
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
		if (ovella == null || (!attached && (ovella.GetComponent<Rigidbody2D>().velocity.magnitude == 0))) {
			attached = false;
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
			Objective=pz;
			pz.z=0;
			Vector3 OP = pz - transform.position;
			OP.z=0;
			Vector3 PivotOvella = transform.position - ovella.transform.position;
			PivotOvella.z=0;
			float R = PivotOvella.magnitude;
			if(OP.magnitude<=R){
				maximumAngle = -60;
			}else{
				float theta = Mathf.Acos (R / OP.magnitude);
				float alpha = Mathf.Atan2 (OP.y, OP.x);
				float beta = (theta + alpha)*180/Mathf.PI;
				Vector3 ini=transform.position;
			
				ini.z=-10;
				Debug.DrawRay(ini,new Vector3(Mathf.Cos(beta),Mathf.Sin (beta))*R);
				maximumAngle=beta-187.41f;
			}

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
				ovella.GetComponent<Rigidbody2D>().isKinematic=false;
				ovella.GetComponent<CircleCollider2D>().isTrigger=false;
				ovella.transform.SetParent(null);
				Vector2 PivotPos = transform.position;
				float speed= (ovella.GetComponent<Rigidbody2D>().position-PivotPos).magnitude*startingAngularSpeed*Mathf.PI/180;
				Vector2 OvellaPos = ovella.transform.position;
				ovella.GetComponent<Rigidbody2D>().velocity = (Objective-OvellaPos).normalized*speed;
				ovella.GetComponent<Rigidbody2D>().angularVelocity = -10;
			}
			if (time < startTime + finalTime)angle = maximumAngle + (time - startTime) * finalAngularSpeed;
		}
		gameObject.GetComponent<Rigidbody2D>().MoveRotation(angle);
	}
}
