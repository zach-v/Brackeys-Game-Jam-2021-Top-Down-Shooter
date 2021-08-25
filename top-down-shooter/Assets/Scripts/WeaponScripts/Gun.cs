using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : WeaponBase
{
	public enum SpecialMethod
	{
		None, SillySound
	}
	public float Damage;
	public float FireRate;
	public bool SingleFire;
	public GameObject WeaponModel;
	public GameObject FireEffect;
	public GameObject TracerEffect;
	public GameObject Impact;
	public SpecialMethod method = SpecialMethod.None;
}
