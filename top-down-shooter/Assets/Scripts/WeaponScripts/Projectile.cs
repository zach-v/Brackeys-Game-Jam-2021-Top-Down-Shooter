using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float Damage = 100;
	public float aoe = 0.1f;
	public float speed = 20;
	public Vector3 LaunchPosition;
	public Vector3 TargetPosition;
	public Transform TargetTransform;
	public bool PersistentFollow = false;
	[ReadOnly] [SerializeField] private Vector3 velocity = new Vector3();
	[ReadOnly] [SerializeField] private Vector3 acceleration = new Vector3();

	private void Update()
	{
		if (PersistentFollow)
			acceleration = Vector3.MoveTowards(transform.position, TargetTransform.position, 1).normalized * speed;
		else
			acceleration = Vector3.MoveTowards(transform.position, TargetPosition, 1).normalized * speed;
	}
	private void FixedUpdate()
	{
		// Physics
		velocity += acceleration;
		acceleration.Set(0, 0, 0);
		transform.position += velocity;
	}
}
