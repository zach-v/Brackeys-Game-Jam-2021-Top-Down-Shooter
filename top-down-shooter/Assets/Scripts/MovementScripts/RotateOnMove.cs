using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnMove : MonoBehaviour
{
	[SerializeField] private float Speed = 0.1f;
	[ReadOnly] [SerializeField] private Vector3 lastPosition;
	[ReadOnly] [SerializeField] private Vector3 direction;
	[ReadOnly] [SerializeField] private Vector3 localDirection;
	[ReadOnly] [SerializeField] private Vector3 rotationEuler;
	void FixedUpdate()
	{
		// Get Direction on movement
		direction = (transform.position - lastPosition).normalized;
		localDirection = transform.InverseTransformDirection(direction);
		lastPosition = transform.position;

		float smooth = Time.deltaTime * Speed * 200;
		rotationEuler = (Quaternion.Euler(0, 90, 0) * direction);
		transform.Rotate(rotationEuler * smooth);
	}
}
