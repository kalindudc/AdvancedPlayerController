using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour {

	public Camera firstPerson;
	public Camera thirdPerson;

	// Use this for initialization
	void Start () {
		thirdPerson.enabled = true;
		firstPerson.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonUp("Camera Mode"))
		{
			// Just toggles
			firstPerson.enabled = !firstPerson.enabled;
			thirdPerson.enabled = !thirdPerson.enabled;
		}
	}

}
