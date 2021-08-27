using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHumanoidRig : MonoBehaviour
{
	[SerializeField] private float rotationSpeed = 5;
	[ReadOnly] [SerializeField] private Vector3 lastPosition;
	[ReadOnly] [SerializeField] private Vector3 newRotation;
	[ReadOnly] [SerializeField] private float mod;

	void Start()
	{
		lastPosition = transform.position;
	}
	// Update is called once per frame
	void LateUpdate()
	{
		mod = Vector3.Distance(lastPosition, transform.position);
		newRotation = new Vector3(transform.eulerAngles.x + mod, transform.eulerAngles.y, transform.eulerAngles.z);
		transform.eulerAngles = newRotation;
		lastPosition = transform.position;
	}
}
