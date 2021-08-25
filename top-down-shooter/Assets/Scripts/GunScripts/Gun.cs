using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun
{
	public string name;
	public float Damage;
	public LayerMask TargetLayer;
	public float FireRate;
	public bool SingleFire;
	public GameObject TracerEffect;
	public GameObject Impact;
	public Action SpecialMethod;
	public void Fire()
	{

	}
}
