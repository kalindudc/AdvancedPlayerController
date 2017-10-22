using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouseLook : MonoBehaviour {

	public float mouseSensitivity = 100.0f;
	public float xClampAngle = 50.0f;
	public float yClampAngle = 45.0f;
    public bool isCursorVisible = false;
	public GameObject player;

	private float rotY = 0.0f; // rotation around the up/y axis
	private float rotX = 0.0f; // rotation around the right/x axis

	// Use this for initialization
	void Start () {
		Vector3 rotation = this.transform.localRotation.eulerAngles;
		rotX = rotation.x;
		rotY = rotation.y;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float mouseX = Input.GetAxis ("Mouse X");
		float mouseY = Input.GetAxis ("Mouse Y");

		rotY += mouseX * mouseSensitivity * Time.deltaTime;
		rotX += mouseY * mouseSensitivity * Time.deltaTime;

		rotX = Mathf.Clamp(rotX, -xClampAngle, xClampAngle);

		Quaternion camRotation = Quaternion.Euler (rotX, rotY, 0.0f);
		Quaternion playerRotation = Quaternion.Euler (0.0f, rotY, 0.0f);

		this.transform.rotation = camRotation;
		player.transform.rotation = playerRotation;
	}

    private void Update()
    {
        if (!isCursorVisible)
        {
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isCursorVisible = true;
        }
        else
        {
            isCursorVisible = false;
        }
    }
}
