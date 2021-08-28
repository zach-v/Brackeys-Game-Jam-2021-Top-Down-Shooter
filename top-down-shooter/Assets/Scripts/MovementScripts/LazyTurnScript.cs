using Assets.Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyTurnScript : MonoBehaviour
{
	[SerializeField] private float TurnSpeed = 0.05f;
	[SerializeField] private Vector2 limitX = new Vector2(-45, 45);
	[SerializeField] private Vector2 limitY = new Vector2(-45, 45);
	[SerializeField] private Vector2 limitZ = new Vector2(-45, 45);
	[ReadOnly] [SerializeField] private Vector3 currentRotation;
	[ReadOnly] [SerializeField] private Vector3 lastPosition;
	[ReadOnly] [SerializeField] private Vector3 direction;
	[ReadOnly] [SerializeField] private Vector3 localDirection;
	void Start()
	{
		lastPosition = transform.position;
	}
	void FixedUpdate()
	{
		// Get Direction of movement
		direction = (transform.position - lastPosition).normalized;
		localDirection = transform.InverseTransformDirection(direction);
		lastPosition = transform.position;

		// Limit turning
		currentRotation = transform.localRotation.eulerAngles;
		currentRotation.x = Extensions.ConvertToAngle180(currentRotation.x);
		currentRotation.y = Extensions.ConvertToAngle180(currentRotation.y);
		currentRotation.z = Extensions.ConvertToAngle180(currentRotation.z);
		// Turn based on direction
		if (localDirection.x > 0.1)
		{
			currentRotation.y = Mathf.Lerp(currentRotation.y, limitY.y, TurnSpeed);
		}
		if (localDirection.x < -0.1)
		{
			currentRotation.y = Mathf.Lerp(currentRotation.y, limitY.x, TurnSpeed);
		}
		// Limit turn
		if (limitX.x != 0 & limitX.y != 0)
			currentRotation.x = Mathf.Clamp(currentRotation.x, limitX.x, limitX.y);
		if (limitY.x != 0 & limitY.y != 0)
			currentRotation.y = Mathf.Clamp(currentRotation.y, limitY.x, limitY.y);
		if (limitZ.x != 0 & limitZ.y != 0)
			currentRotation.z = Mathf.Clamp(currentRotation.z, limitZ.x, limitZ.y);
		transform.localRotation = Quaternion.Euler(currentRotation);
	}

}
