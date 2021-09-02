using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun : WeaponBase
{
	public enum SpecialMethod
	{
		None, SillySound
	}
	[Header("Fire Parameters")]
	public float Damage = 1;
	public int ShotCount = 1;
	public float spread = 0.01f; // In percentage
	public float Range = 50;
	public bool SingleFire = false;
	public bool HitScan = true;
	public string FireSoundName = "pistol-1";
	[Header("Game Objects")]
	public GameObject WeaponModel;
	public Projectile projectile;
	public GameObject FireEffect;
	public float FireEffectTime = 3;
	public GameObject TracerEffect;
	public float TracerEffectTime = 3;
	public GameObject ImpactEffect;
	public float ImpactEffectTime = 3;
	[Header("Weapon Handling")]
	public bool UseCustomHandPositions = false;
	public GameObject muzzle;
	public GameObject RightHand;
	public GameObject LeftHand;
	public bool SingleHanded = true;
	[ReadOnly] public List<GameObject> ActiveEffects;
	[Header("Other")]
	public SpecialMethod method = SpecialMethod.None;
	public Gun()
	{
		ActiveEffects = new List<GameObject>();
	}
	public override void Method()
	{
		switch (method)
		{
			default:
				break;
		}
	}
	public Vector3 Spread(Vector3 aim, float distance, float variance)
	{
		aim.Normalize();
		Vector3 v3;
		do
		{
			v3 = UnityEngine.Random.insideUnitSphere;
		} while (v3 == aim || v3 == -aim);
		v3 = Vector3.Cross(aim, v3);
		v3 = v3 * UnityEngine.Random.Range(0.0f, variance);
		return aim * distance + v3;
	}
}
