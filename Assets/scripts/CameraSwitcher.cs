using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour {

	public bool isFirstPerson = false;

	public Camera firstPerson;
	public Camera thirdPerson;

	// Use this for initialization
	void Start () {
		thirdPerson.enabled = !isFirstPerson;
		firstPerson.enabled = isFirstPerson;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonUp("Camera Mode"))
		{
			// Just toggles
			isFirstPerson = !isFirstPerson;
			firstPerson.enabled = isFirstPerson;
			thirdPerson.enabled = !isFirstPerson;
		}
	}

}
