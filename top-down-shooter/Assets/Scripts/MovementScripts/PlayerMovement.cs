using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public CharacterController controller;
	public Camera cam;
	public float moveSpeed = 5f;
	[Range(0, 1)] public float accelerationTime = 0.15f;
	public float degreeOffset = 90f;

	[Header("Internal Variables")]
	private float currentSpeed = 0;
	[ReadOnly] [SerializeField] private Vector2 movement;
	[ReadOnly] [SerializeField] private Vector3 mousePos;
	[ReadOnly] [SerializeField] private float angle = 0;
	[ReadOnly] [SerializeField] private string DisplaySpeed = "0.0";

	void Update()
	{
		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");

		mousePos = Input.mousePosition;
	}

	void FixedUpdate()
	{
		// Movement
		if (movement.magnitude >= 0.1f)
		{
			Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized;
			controller.Move(direction * Mathf.Lerp(currentSpeed, moveSpeed, accelerationTime) * Time.fixedDeltaTime);
			currentSpeed = controller.velocity.magnitude;
			DisplaySpeed = currentSpeed.ToString("0.0");
		}

		Ray rayFromMouse = cam.ScreenPointToRay(mousePos);
		if (Physics.Raycast(rayFromMouse, out RaycastHit hitInfo, 300f))
		{
			Vector3 lookDir = hitInfo.point - controller.transform.position;

			angle = Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg - degreeOffset;

			controller.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));
		}
	}
}
