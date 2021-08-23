using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    public float Damage { get; }
    public LayerMask TargetLayer { get; }
    public float FireRate { get; }
    public bool SingleFire { get; }
    public GameObject TracerEffect { get; }
    public GameObject Impact { get; }
    public Action SpecialMethod { get; }
    public void Fire()
	{

	}
	private void FixedUpdate()
	{
		
	}
}
