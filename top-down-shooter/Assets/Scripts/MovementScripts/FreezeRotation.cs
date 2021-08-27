using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
	[SerializeField] private bool X = true;
	[SerializeField] private bool Y = true;
	[SerializeField] private bool Z = true;
	[ReadOnly] [SerializeField] private Vector3 initialRotation;
	void Awake()
	{
		initialRotation = transform.rotation.eulerAngles;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (!X)
			initialRotation.x = transform.eulerAngles.x;
		if (!Y)
			initialRotation.y = transform.eulerAngles.y;
		if (!Z)
			initialRotation.z = transform.eulerAngles.z;
		transform.eulerAngles = initialRotation;
	}
}
